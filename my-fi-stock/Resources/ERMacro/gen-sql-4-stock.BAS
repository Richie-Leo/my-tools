Dim mdl As Model,submdl As SubModel, constructor As String

Sub Main
	Dim file
	Dim names As Variant ,count As Variant, i As Integer, j As Integer, column As String, dataType As String
	Dim ent As Entity, attr As AttributeObj

	Set mdl = DiagramManager.ActiveDiagram.ActiveModel
	Set submdl = mdl.ActiveSubModel
	submdl.EntityNames(names, count)

			Set file = CreateObject("ADODB.Stream")
			file.open()
			file.Charset = "utf-8"
			file.Position = 0

	For i=0 To count -1
		Set ent = mdl.Entities.Item(names(i))

		If Not(ent.Definition="ignore" Or ent.Definition="ignored") And InStr(ent.TableName,".")<=0 Then
	
			writeFile(file, "DROP TABLE IF EXISTS `" & ent.TableName & "`;" & vbCrLf)
			writeFile(file, "CREATE TABLE `" & ent.TableName & "` (" & vbCrLf)
			For j = 1 To ent.Attributes.Count
				Set attr = findAttribute(ent.Attributes, j)
				If Not(attr.Definition = "ignore" Or attr.Definition = "ignored" Or attr.Definition = "calc_prop") Then
					dataType = toMysqlDatatype(attr.Datatype)
					column = vbTab & "`" & attr.ColumnName & "` " & dataType
					If attr.DataScale > 0 Then
						column = column & "(" & attr.DataLength & "," & attr.DataScale & ")"
					ElseIf attr.DataLength>0 Then
						column = column & "(" & attr.DataLength & ")"
					End If
		
					If attr.NullOption="NULL" Then
						column = column & " NULL"
					Else
						column = column & " NOT NULL"
					End If
		
					If attr.Identity Then
						column = column & " AUTO_INCREMENT"
					End If

					If Not attr.Identity Then '自增类型的字段不能设置默认值
						If dataType = "timestamp" Then
							column = column & " DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"
						ElseIf attr.DeclaredDefault<>"" Then
							column = column & " DEFAULT " & attr.DeclaredDefault
						End If
					End If
		
					If Len(attr.Notes)>0 Then
						column = column & " COMMENT '" & getSqlComments(attr.Notes) & "'"
					End If
		
					If j<>ent.Attributes.Count Then
						column = column & ","
					End If
					writeFile(file, column & vbCrLf)
				End If
			Next
	
			column = primaryKeys(ent)
			If Len(column)>0 Then
				writeFile(file, column)
			End If
	
			writeFile(file, ") ENGINE=InnoDB DEFAULT CHARSET=utf8")
			If Len(ent.Note)>0 Then
				writeFile(file, " COMMENT '" & getSqlComments(ent.Note) & "'")
			End If
	
			writeFile(file, ";" & vbCrLf)
		End If
	Next

			file.SaveToFile("D:\stock\tools\er-workspace\" & submdl.Name & ".sql", 2)
			file.Close()
End Sub

Function getSqlComments(s As String) As String
	If (Len(Trim(s))<=0) Then
		getSqlComments = ""
		Exit Function
	End If
	' 备注中包含回车换行，只取第一行内容
	If InStr(s, vbCrLf)>0 Then
		getSqlComments = Split(s, vbCrLf)(0)
		Exit Function
	End If
	getSqlComments = s
End Function


Function toMysqlDatatype(dt As String) As String
	If dt = "INTEGER" Then
		toMysqlDatatype = "int"
		Exit Function
	End If
	If dt = "TIMESTAMP/DATE" Then
		toMysqlDatatype = "timestamp"
		Exit Function
	End If
	If dt = "TIME/DATETIME" Then
	End If
	toMysqlDatatype = LCase(dt)
End Function

Function primaryKeys(ent As Entity)
	Dim keys As String, keysSQL As String, attr As AttributeObj, ind As Index, column As IndexColumn, num As Integer
	keysSQL = ""
	For Each ind In ent.Indexes
		keysSQL = keysSQL & vbTab & ","
		If ind.IsPK Then
			keysSQL = keysSQL & " PRIMARY KEY"
		Else
			If LCase(Mid(ind.Name, 1, 3))="uk_" Or LCase(Mid(ind.Name, 1, 3))="ux_" Then
				keysSQL = keysSQL & " UNIQUE"
			End If
			keysSQL = keysSQL & " KEY `" & ind.Name & "`"
		End If
		keysSQL = keysSQL & "("
		num = 0
		For Each column In ind.IndexColumns
			If num>0 Then
				keysSQL = keysSQL & ","
			End If
			keysSQL = keysSQL & "`" & column.ColumnName & "`"
			num = num + 1
		Next
		keysSQL = keysSQL & ")" & vbCrLf
	Next

	primaryKeys = keysSQL
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

Function writeFile(file As Variant, aLine As String )
	' 用ADODB.Stream
	file.WriteText(aLine)
	' 用Scripting.FileSystemObject
	'file.WriteLine(aLine)
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
