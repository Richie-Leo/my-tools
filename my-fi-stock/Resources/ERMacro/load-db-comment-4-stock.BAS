Dim m As Model, sm As SubModel
Dim dbServer As String , dbInfoSchemaName As String, dbTargetSchemaName As String , dbUsr As String , dbPsw As String
Dim cn, rs

Sub Main
	Dim ent As Entity, attr As AttributeObj
	Dim i As Integer, j As Integer, names As Variant, comment As String ,count As Variant, s As String

	dbServer = "local-mac"
	dbInfoSchemaName = "information_schema"
	dbUsr = "root"
	dbPsw = "dev"
	dbTargetSchemaName = "my_fi_stock"

	Set m = DiagramManager.ActiveDiagram.ActiveModel
	Set sm = m.ActiveSubModel
	sm.EntityNames(names, count) ' 读取实体名称放入names（数组）中，实体数量放入count（整型）中
	Call quickSort(names) ' 按字母排序Entities

	Set cn = CreateObject("ADODB.Connection")
	Set rs = CreateObject("ADODB.Recordset")
	cn.connectionstring = "Driver={MySQL ODBC 5.2w Driver};Server=" & dbServer & ";Database=" & dbInfoSchemaName & ";User=" & dbUsr & "; Password=" & dbPsw & ";"
	cn.open

	For i=0 To count -1
		Set ent = m.Entities.Item(names(i))
		s = findTableComments(rs, ent.TableName)

		If s <> "NOT EXISTS" Then
			ent.Note = s
			rs.Open "select * from COLUMNS where table_schema='" & dbTargetSchemaName & "' and table_name='" & ent.TableName & "'", cn, 3
			For j = 1 To ent.Attributes.Count
				Set attr = findAttribute(ent.Attributes, j)
				comment = findColumnComments(rs, attr.ColumnName)
				attr.Notes = comment
			Next
			rs.Close
		End If
	Next

	cn.Close
End Sub

Function findTableComments(rs As Variant, tableName As String) As String
	rs.Open "select * from TABLES where table_schema='" & dbTargetSchemaName & "' and table_name='" & tableName & "'", cn, 3
	If rs Is Nothing Or rs.EOF Then
		findTableComments = "NOT EXISTS"
		rs.Close
		Exit Function
	End If
	rs.MoveFirst
	While Not rs.EOF
		findTableComments = rs.Fields.Item("TABLE_COMMENT").Value
		rs.Close
		Exit Function
	Wend
	findTableComments = ""
	rs.Close
End Function


Function findColumnComments(rs As Variant, columnName As String) As String
	findColumnComments = ""
	If rs Is Nothing Then
		Exit Function
	End If
	rs.MoveFirst
	While Not rs.EOF
		If rs.Fields.Item("COLUMN_NAME").Value=columnName Then
			findColumnComments = rs.Fields.Item("COLUMN_COMMENT").Value
			Exit Function
		End If
	    rs.MoveNext
	Wend
End Function

' 根据索引查找Entity
Function findAttribute(attrs As Attributes, i As Integer) As AttributeObj
	Dim result As AttributeObj
	Dim attr As AttributeObj
	Set result = Nothing
	For Each attr In attrs
		If attr.SequenceNumber=i Then
			Set findAttribute=attr
			Exit Function
		End If
	Next
	Set findAttribute=Nothing
End Function

Const dhcMissing = -2
' 冒泡排序，按照AttributeName的字母顺序排序
Sub quickSort(varArray As Variant, Optional intLeft As Integer = dhcMissing, Optional intRight As Integer = dhcMissing)
    Dim i As Integer
    Dim j As Integer
    Dim varTestVal As Variant
    Dim intMid As Integer

    If intLeft = dhcMissing Then intLeft = LBound(varArray)
    If intRight = dhcMissing Then intRight = UBound(varArray)

    If intLeft < intRight Then
        intMid = (intLeft + intRight) \ 2
        varTestVal = UCase(varArray(intMid))
        i = intLeft
        j = intRight
        Do
            Do While UCase(varArray(i)) < varTestVal
                i = i + 1
            Loop
            Do While UCase(varArray(j)) > varTestVal
                j = j - 1
            Loop
            If i <= j Then
                SwapElements varArray, i, j
                i = i + 1
                j = j - 1
            End If
        Loop Until i > j
        If j <= intMid Then
            Call quickSort(varArray, intLeft, j)
            Call quickSort(varArray, i, intRight)
        Else
            Call quickSort(varArray, i, intRight)
            Call quickSort(varArray, intLeft, j)
        End If
    End If
End Sub

Private Sub SwapElements(varItems As Variant, intItem1 As Integer, intItem2 As Integer)
    Dim varTemp As Variant

    varTemp = varItems(intItem2)
    varItems(intItem2) = varItems(intItem1)
    varItems(intItem1) = varTemp
End Sub
