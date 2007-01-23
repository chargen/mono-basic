' 
' Visual Basic.Net Compiler
' Copyright (C) 2004 - 2007 Rolf Bjarne Kvinge, RKvinge@novell.com
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This library is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
' Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public
' License along with this library; if not, write to the Free Software
' Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
' 

''' <summary>
''' MidAssignmentStatement  ::=
'''	   "Mid" [ "$" ]  "("  Expression "," Expression  [ "," Expression  ] ")"  =  Expression  
''' </summary>
''' <remarks></remarks>
Public Class MidAssignStatement
    Inherits Statement

    'Mid(Target, Start, [Length]) = Source
    Private m_Target As Expression
    Private m_Start As Expression
    Private m_Length As Expression
    Private m_Source As Expression

    Sub New(ByVal Parent As ParsedObject)
        MyBase.New(Parent)
    End Sub

    Sub Init(ByVal Target As Expression, ByVal Start As Expression, ByVal Length As Expression, ByVal Source As Expression)
        m_Target = Target
        m_Start = Start
        m_Length = Length
        m_Source = Source
    End Sub

    Friend Overrides Function GenerateCode(ByVal Info As EmitInfo) As Boolean
        Dim result As Boolean = True

        Helper.Assert(m_Target.Classification.IsVariableClassification)
        Helper.Assert(m_Start.Classification.CanBeValueClassification)
        Helper.Assert(m_Length Is Nothing OrElse m_Length.Classification.CanBeValueClassification)
        Helper.Assert(m_Source.Classification.CanBeValueClassification)

        result = m_Target.GenerateCode(Info.Clone(True, False, Compiler.TypeCache.String_ByRef)) AndAlso result

        result = m_Start.GenerateCode(Info.Clone(True, False, Compiler.TypeCache.Integer)) AndAlso result
        Emitter.EmitConversion(Compiler.TypeCache.Integer, Info)
        If m_Length IsNot Nothing Then
            result = m_Length.GenerateCode(Info.Clone(True, False, Compiler.TypeCache.Integer)) AndAlso result
            Emitter.EmitConversion(Compiler.TypeCache.Integer, Info)
        Else
            Emitter.EmitLoadI4Value(Info, Integer.MaxValue)
        End If
        result = m_Source.GenerateCode(Info.Clone(True, False, Compiler.TypeCache.String)) AndAlso result
        Emitter.EmitConversion(Compiler.TypeCache.String, Info)

        Emitter.EmitCallOrCallVirt(Info, Compiler.TypeCache.MS_VB_CS_StringType_MidStmtStr__String_Integer_Integer_String)

        Return result
    End Function

    Public Overrides Function ResolveStatement(ByVal Info As ResolveInfo) As Boolean
        Dim result As Boolean = True

        result = m_Target.ResolveExpression(Info) AndAlso result
        result = m_Start.ResolveExpression(info) AndAlso result
        If m_Length IsNot Nothing Then result = m_Length.ResolveExpression(Info) AndAlso result
        result = m_Source.ResolveExpression(info) AndAlso result

        Compiler.Helper.AddCheck("The first argument is the target of the assignment and must be classified as a variable or a property access whose type is implicitly convertible to and from String. ")
        Compiler.Helper.AddCheck("The second parameter is the 1-based start position that corresponds to where the assignment should begin in the target string and must be classified as a value whose type must be implicitly convertible to Integer")
        Compiler.Helper.AddCheck("The optional third parameter is the number of characters from the right-side value to assign into the target string and must be classified as a value whose type is implicitly convertible to Integer")
        Compiler.Helper.AddCheck("The right side is the source string and must be classified as a value whose type is implicitly convertible to String.")

        Return result
    End Function

    Shared Function IsMe(ByVal tm As tm) As Boolean
        Return tm.CurrentToken = "Mid" AndAlso tm.PeekToken = KS.LParenthesis
    End Function
#If DEBUG Then
    Public Sub Dump(ByVal Dumper As IndentedTextWriter)
        dumper.Write("Mid(")
        m_Target.Dump(dumper)
        dumper.Write(", ")
        m_Start.Dump(Dumper)
        If m_Length IsNot Nothing Then
            dumper.Write(", ")
            m_Length.Dump(Dumper)
        End If
        dumper.Write(") = ")
        m_Source.Dump(Dumper)
        Dumper.WriteLine("")
    End Sub
#End If
End Class