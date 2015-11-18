Dim mdl As Model,submdl As SubModel, constructor As String

Sub Main
	Dim file
	Dim names As Variant ,count As Variant, i As Integer, j As Integer, enumName As String, block As String
	Dim ent As Entity, attr As AttributeObj

	Set mdl = DiagramManager.ActiveDiagram.ActiveModel
	Set submdl = mdl.ActiveSubModel
	submdl.EntityNames(names, count)

	Randomize() '初始化随机序列的种子值
	For i=0 To count -1
		Set ent = mdl.Entities.Item(names(i))
		If Not( ent.Definition = "ignore" Or ent.Definition = "ignored") Then
			Set file = CreateObject("ADODB.Stream")
			file.Type = 2 ' text
			file.Mode = 3 ' read/write
			file.Charset = "UTF-8"
			file.open()
			file.Position = 0
	
			' 命名空间，using语句
			block = "using System;" & vbCrLf
			block = block & "using System.Data;" & vbCrLf
			block = block & "using Pandora.Basis.DB;" & vbCrLf
			block = block & vbCrLf
			block = block & "namespace Pandora.Invest.Entity" & vbCrLf
			block = block & "{" & vbCrLf
			writeFile(file, block)
			' 类注释，类定义
			block = vbTab & "/// <summary>" & vbCrLf
			If Len(ent.Note)>0 Then
				block = block & GetComments(ent.Note, 3)
			End If
			block = block & vbTab & "public partial class " & toEntityName(ent.EntityName) & vbCrLf
			block = block & vbTab & "{" & vbCrLf
			writeFile(file, block)
			' Mapper定义
			block = vbTab & vbTab & "/// <summary>" & vbCrLf
			block = block & vbTab & vbTab & "/// 实体属性->数据表字段 映射" & vbCrLf
			block = block & vbTab & vbTab & "/// </summary>" & vbCrLf
			block = block & vbTab & vbTab & "public static class Mapper" & vbCrLf
			block = block & vbTab & vbTab & "{" & vbCrLf
			block = block & vbTab & vbTab & vbTab & "public const string TableName = """ & ent.TableName & """;" & vbCrLf & vbCrLf
			For j = 1 To ent.Attributes.Count
				Set attr = findAttribute(ent.Attributes, j)
				If Not (attr.Definition="ignore" Or attr.Definition="ignored") Then
					block = block & vbTab & vbTab & vbTab & "public const string " & toPropertyName(attr.LogicalRoleName) & _
						" = """ & attr.ColumnName & """;" & vbCrLf
				End If
			Next
			block = block & vbTab & vbTab & "}" & vbCrLf
			writeFile(file, block)
			' 属性定义
			block = ""
			For j = 1 To ent.Attributes.Count
				Set attr = findAttribute(ent.Attributes, j)
				If Not (attr.Definition="ignore" Or attr.Definition="ignored") Then
					If Len(attr.Notes)>0 Then
						block = block & vbCrLf & GetComments(attr.Notes, 1)
					End If
					block = block & vbTab & vbTab & "public " & GetDomainType(attr, "domain") & _
						" " & toPropertyName(attr.LogicalRoleName) & " { get; set; }" & vbCrLf
				End If
			Next
			writeFile(file, block)
			' 带DataRow的构造函数
			block = vbCrLf & vbTab & vbTab & "public " & toEntityName(ent.EntityName) & "() {}" & vbCrLf
			block = block & vbTab & vbTab & "private " & toEntityName(ent.EntityName) & "(DataRow row) {" & vbCrLf
			For j=1 To ent.Attributes.Count
				Set attr = findAttribute(ent.Attributes, j)
				block = block & vbTab & vbTab & vbTab & _
					"this." & toPropertyName(attr.LogicalRoleName) & " = " & _
					GetDomainType(attr, "cast") & "(row[Mapper." & toPropertyName(attr.LogicalRoleName) & "]);" & vbCrLf
			Next
			block = block & vbTab & vbTab & "}" & vbCrLf
			writeFile(file, block)
			' BulkInserter
			block = vbCrLf & vbTab & vbTab & "public class " & toEntityName(ent.EntityName) & "BulkInserter<T> : BulkInserter<T>{" & vbCrLf
			block = block & vbTab & vbTab & vbTab & "public " & _
				toEntityName(ent.EntityName) & "BulkInserter(Database db, int batchSize) : base(db, Mapper.TableName, new string[] {" & vbCrLf
			For j=1 To ent.Attributes.Count
				Set attr = findAttribute(ent.Attributes, j)
				If j Mod 5 = 1 Then ' 一行5个属性
					If j>1 Then
						block = block & vbCrLf
					End If
					block = block & vbTab & vbTab & vbTab & vbTab & vbTab
				End If
				If Not attr.Identity Then
					block = block & "Mapper." & toPropertyName(attr.LogicalRoleName)
					If j<ent.Attributes.Count Then
						block = block & ", "
					End If
				End If
			Next
			block = block & vbCrLf & vbTab & vbTab & vbTab & vbTab & "}, batchSize) {}" & vbCrLf
			block = block & vbCrLf & vbTab & vbTab & vbTab & "public override BulkInserter<T> Push(T obj){" & vbCrLf
			block = block & vbTab & vbTab & vbTab & vbTab & toEntityName(ent.EntityName) & " e = obj as " & toEntityName(ent.EntityName) & ";" & vbCrLf
			block = block & vbTab & vbTab & vbTab & vbTab & _
				"if(e == null) throw new EntityException(""The type of obj is not " & toEntityName(ent.EntityName) & """);" & vbCrLf
			block = block & vbTab & vbTab & vbTab & vbTab & "base.Push(new object[] {" & vbCrLf
			For j=1 To ent.Attributes.Count
				Set attr = findAttribute(ent.Attributes, j)
				If j Mod 5 = 1 Then ' 一行5个属性
					If j>1 Then
						block = block & vbCrLf
					End If
					block = block & vbTab & vbTab & vbTab & vbTab & vbTab
				End If
				If Not attr.Identity Then
					block = block & "e." & toPropertyName(attr.LogicalRoleName)
					If j<ent.Attributes.Count Then
						block = block & ", "
					End If
				End If
			Next
			block = block & vbCrLf & vbTab & vbTab & vbTab & vbTab & "});" & vbCrLf
			block = block & vbTab & vbTab & vbTab & vbTab & "return this;" & vbCrLf
			block = block & vbTab & vbTab & vbTab & "}" & vbCrLf
			block = block & vbTab & vbTab & "}" & vbCrLf
			writeFile(file, block)
	
			writeFile(file, vbTab & "}" & vbCrLf & "}")
			saveFile(file, "D:\stock\tools\ER-workspace\" & toEntityName(ent.EntityName) & ".cs")
		End If
	Next
End Sub

' mode=1, 为属性生成注释; mode=3, 为类生成注释
Function GetComments(s As String, mode As Integer) As String
	Dim result As String, h As String, t As String, c As String, arr As Variant, i As Integer, j As Integer

	If Len(s)<=0 Then
		GetComments = ""
		Exit Function
	End If
	
	arr = Split(s, vbCrLf)
	If UBound(arr)>0 Then
		h = arr(0) '标题性质
		t = Mid(s, Len(h)+1, Len(s) - Len(h))
		'把Notes中去掉标题行后，行首的回车换行符去掉，避免后面产生多余的<p>
		Do While Mid(t, 1, 2)= vbCrLf '前2个字符为回车换行
			t = Mid(t, 3, Len(t) - 2) '从第3个字符开始截取
		Loop
		'把行首的空格进行HTML转义处理
		arr = Split(t, vbCrLf)
		If UBound(arr)>0 Then
			i = 0
			Do While i<=UBound(arr)
				j = 1
				If Len(arr(i))>0 Then
					Do While j<=Len(arr(i))
						If Mid(arr(i), j, 1)<>" " Then
							Exit Do
						End If
						j = j+1
					Loop
				End If
				If j>1 Then
					arr(i) = Replace(arr(i), " ", "&nbsp;", 1, j-1)
				End If
				i = i+1
			Loop
			t = ""
			i=0
			Do While i<=UBound(arr)
				t = t & arr(i) & vbCrLf
				i = i+1
			Loop
		End If
	Else
		h = s
		t = ""
	End If
	'类的JavaDoc，以句号结束会作为类的概要描述，后面的部分则作为详细描述，因此特意在末尾添加一个句号。
	c = Mid(h, Len(h),1)
	If c<>"." And c<>"。" Then 
		h = h & "。"
	End If
	Do While True ' 去掉行尾的回车换行
		If Len(t)>2 Then
			If Mid(t, Len(t)-1, 2) = vbCrLf Then
				t = Mid(t, 1, Len(t)-2)
			Else
				Exit Do
			End If
		Else
			Exit Do
		End If
	Loop

	If mode=1 Then '
		result = result & vbTab & vbTab & "/// <summary>" & vbCrLf
		result = result & vbTab & vbTab & "/// " & h & vbCrLf
	Else
		result = result & vbTab & "/// " & h & vbCrLf
	End If
	
	If Len(t)>0 Then
		t = Replace(t, vbCrLf & vbCrLf, "<==DOUBLE-CRLF==>")
		If mode=1 Then
			result = result & vbTab & vbTab & "/// " & replaceCrlf(t, "<br />" & vbCrLf & vbTab & vbTab & "///") & vbCrLf
			result = Replace(result, "<==DOUBLE-CRLF==>", vbCrLf & vbTab & vbTab & "///" & vbCrLf & vbTab & vbTab & "/// ")
		Else
			result = result & vbTab & "/// " & replaceCrlf(t, "<br />" & vbCrLf & vbTab & "///") & vbCrLf
			result = Replace(result, "<==DOUBLE-CRLF==>", vbCrLf & vbTab & "///" & vbCrLf & vbTab & "/// <p>" & vbCrLf & vbTab & "/// ")
		End If
	End If

	If mode=1 Then
		result = result & vbTab & vbTab & "/// </summary>" & vbCrLf
	Else
		result = result & vbTab & "/// </summary>" & vbCrLf
	End If

	GetComments = result
End Function

Function findAttribute(attrs As Attributes, Index As Integer)
	Dim result As AttributeObj
	Dim attr As AttributeObj
	Set result = Nothing
	For Each attr In attrs
		If attr.SequenceNumber=Index Then
			Set findAttribute=attr
			Exit Function
		End If
	Next
	Set findAttribute=Nothing
End Function

Function replaceCrlf(value As String, rep As String)
	If Len(value)<=0 Then
		replaceCrlf = ""
		Exit Function
	End If
	replaceCrlf = Replace(value, vbCrLf, rep)
End Function

' 获取.NET类型相关
Function GetDomainType(attr As AttributeObj, reqType As String) As String
	Dim dt As String, domainType As String, castFunc As String
	' 普通类型
	dt = attr.Datatype
	If dt="DECIMAL" Or dt="DOUBLE PRECISION" Then
		domainType = "decimal"
		castFunc = "Convert.ToDecimal"
	ElseIf InStr(attr.LogicalRoleName, "Flag") Or Trim(LCase(attr.Definition))="bool" Or Trim(LCase(attr.Definition)) ="boolean" Then
		domainType = "bool"
		castFunc = "Convert.ToBoolean"
	ElseIf dt="CHAR" Or dt="VARCHAR" Or dt="NVARCHAR" Or dt="NCHAR" Or dt="TEXT" Or dt="NTEXT" Or dt="LONG TEXT" Then
		domainType = "string"
		castFunc = "Convert.ToString"
	ElseIf dt="INTEGER" Or dt="TINYINT" Or dt="SMALLINT" Then
		domainType = "int"
		castFunc = "Convert.ToInt32"
	ElseIf dt="BIGINT" Then
		domainType = "long"
		castFunc = "Convert.ToInt64"
	ElseIf dt="DATE" Or dt = "DATETIME" Or dt="TIMESTAMP/DATE" Or dt = "TIME/DATETIME" Then
		domainType = "DateTime"
		castFunc = "Convert.ToDateTime"
	Else
		domainType = "===UnsupportedDataType==="
		castFunc = "===UnsupportedDataType==="
	End If

	If reqType="domain" Then
		GetDomainType = domainType
	ElseIf reqType="cast" Then
		GetDomainType = castFunc
	Else
		GetDomainType = "===UnkonwnReqTypeForGetDomainType==="
	End If
End Function


Function toPropertyName(attrName As String) As String
	attrName = replaceUnderScore(attrName)
	toPropertyName = UCase(Mid(attrName, 1, 1)) & Mid(attrName, 2)
End Function

Function toFieldName(attr As AttributeObj) As String
	Dim attrName As String
	If Len(Trim(attr.LogicalRoleName))>0 Then
		attrName = Trim(attr.LogicalRoleName)
	Else
		attrName = attr.AttributeName
	End If
	attrName = replaceUnderScore(attrName)
	toFieldName = LCase(Mid(attrName, 1, 1)) & Mid(attrName, 2)
End Function
Function toEntityName(attrName As String) As String
	attrName = replaceUnderScore(attrName)
	toEntityName = UCase(Mid(attrName, 1, 1)) & Mid(attrName, 2)
End Function

Function replaceUnderScore(n As String ) As String
	Dim arr As Variant, i As Integer, r As String
	arr = Split(n, "_")
	r = ""
	For i=LBound(arr) To UBound(arr)
		r = r & UCase(Mid(arr(i) ,1,1)) & Mid(arr(i), 2)
	Next
	replaceUnderScore=r
End Function

' erstudio宏没有split函数，自行实现一个
Function Split(ByVal sstr As String, ByVal spstr As String) As Variant
	Dim star, lenstr, lensp, cur As Integer
	Dim backstr() As String
	Dim i As Integer
	ReDim backstr(0)
	lenstr = Len(sstr)
	lensp = Len(spstr)
	star = InStr(sstr, spstr)
	If star < 1 Then
	    backstr(0) = sstr
	    Split = backstr()
	    Exit Function
	End If
	backstr(0) = Left(sstr, star - 1)
	cur = star + lensp
	For i = star + lensp To lenstr
	    star = InStr(star + lensp, sstr, spstr)
	    If star > 0 Then
	        ReDim Preserve backstr(UBound(backstr) + 1)
	        backstr(UBound(backstr)) = Mid(sstr, cur, star - cur)
	        cur = star + lensp
	    Else
	        Exit For
	    End If
	Next
	ReDim Preserve backstr(UBound(backstr) + 1)
	backstr(UBound(backstr)) = Mid(sstr, cur, lenstr - cur + 1)
	Split = backstr()
End Function

Function writeFile(file As Variant, aLine As String )
	file.WriteText(aLine)
End Function
Function saveFile(file As Variant, path As String)
	Dim target As Object
	Set target = CreateObject("ADODB.Stream")
	target.Type = 1 ' binary
	target.Mode = 3 ' read/write
	target.open()
	file.Position = 3 ' skip UTF-8 BOM (3 bytes)
	file.CopyTo target
	target.SaveToFile(path, 2)
	target.Flush()
	target.Close()

	file.Flush()
	file.Close()
End Function
