' 
' Visual Basic.Net Compiler
' Copyright (C) 2004 - 2010 Rolf Bjarne Kvinge, RKvinge@novell.com
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

Public Class CSByteExpression
    Inherits ConversionExpression

    Sub New(ByVal Parent As ParsedObject, ByVal Expression As Expression)
        MyBase.New(Parent, Expression)
    End Sub

    Sub New(ByVal Parent As ParsedObject)
        MyBase.New(Parent)
    End Sub

    Protected Overrides Function GenerateCodeInternal(ByVal Info As EmitInfo) As Boolean
        Return GenerateCode(Me.Expression, Info)
    End Function

    Protected Overrides Function ResolveExpressionInternal(ByVal Info As ResolveInfo) As Boolean
        Dim result As Boolean = True

        result = MyBase.ResolveExpressionInternal(Info) AndAlso result

        result = Validate(Info, Expression.ExpressionType) AndAlso result

        Return result
    End Function

    Shared Function Validate(ByVal Info As ResolveInfo, ByVal SourceType As Mono.Cecil.TypeReference) As Boolean
        Dim result As Boolean = True

        Dim expType As Mono.Cecil.TypeReference = SourceType
        Dim expTypeCode As TypeCode = Helper.GetTypeCode(Info.Compiler, expType)
        Dim ExpressionType As Mono.Cecil.TypeReference = Info.Compiler.TypeCache.System_SByte
        Select Case expTypeCode
            Case TypeCode.Char
                Info.Compiler.Report.ShowMessage(Messages.VBNC32006, expType.Name)
                result = False
            Case TypeCode.DateTime
                Info.Compiler.Report.ShowMessage(Messages.VBNC30311, expType.Name, expType.Name)
                result = False
        End Select

        Return result
    End Function


    Overloads Shared Function GenerateCode(ByVal Expression As Expression, ByVal Info As EmitInfo) As Boolean
        Dim result As Boolean = True

        Dim expType As Mono.Cecil.TypeReference = Expression.ExpressionType
        Dim expTypeCode As TypeCode = Helper.GetTypeCode(info.Compiler, expType)

        result = Expression.Classification.GenerateCode(Info.Clone(Expression, expType)) AndAlso result

        Select Case expTypeCode
            Case TypeCode.Boolean
                Emitter.EmitLoadI4Value(Info, 0I, expType)
                Emitter.EmitGT_Un(Info, expType)
                Emitter.EmitNeg(Info)
                Emitter.EmitConv_I1(Info, expType)
            Case TypeCode.SByte
                'Nothing to do
            Case TypeCode.Char
                Info.Compiler.Report.ShowMessage(Messages.VBNC32006, expType.Name)
                result = False
            Case TypeCode.DateTime
                info.Compiler.Report.ShowMessage(Messages.VBNC30311, expType.Name, exptype.Name)
                result = False
            Case TypeCode.Int16, TypeCode.Int32, TypeCode.Int64
                Emitter.EmitConv_I1_Overflow(Info, expType)
            Case TypeCode.Byte, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64
                Emitter.EmitConv_I1_Overflow_Underflow(Info, expType)
            Case TypeCode.Double
                Emitter.EmitCall(Info, Info.Compiler.TypeCache.System_Math__Round_Double)
                Emitter.EmitConv_I1_Overflow(Info, expType)
            Case TypeCode.Single
                Emitter.EmitConv_R8(Info, expType)
                Emitter.EmitCall(Info, Info.Compiler.TypeCache.System_Math__Round_Double)
                Emitter.EmitConv_I1_Overflow(Info, Info.Compiler.TypeCache.System_Double)
            Case TypeCode.Object
                If Helper.CompareType(expType, Info.Compiler.TypeCache.System_Object) Then
                    Emitter.EmitCall(Info, Info.Compiler.TypeCache.MS_VB_CS_Conversions__ToSByte_Object)
                Else
                    Return Info.Compiler.Report.ShowMessage(Messages.VBNC99997, Expression.Location)
                End If
            Case TypeCode.String
                Emitter.EmitCall(Info, Info.Compiler.TypeCache.MS_VB_CS_Conversions__ToSByte_String)
            Case TypeCode.Decimal
                Emitter.EmitCall(Info, Info.Compiler.TypeCache.System_Convert__ToSByte_Decimal)
            Case Else
                Return Info.Compiler.Report.ShowMessage(Messages.VBNC99997, Expression.Location)
        End Select

        Return result
    End Function

    Public Overrides ReadOnly Property ConstantValue() As Object
        Get
            Dim tpCode As TypeCode
            Dim originalValue As Object
            originalValue = Expression.ConstantValue
            tpCode = Helper.GetTypeCode(Compiler, CecilHelper.GetType(Compiler, originalValue))
            Select Case tpCode
                Case TypeCode.SByte
                    Return CSByte(originalValue) 'No range checking needed.
                Case TypeCode.Boolean, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.UInt64, TypeCode.Int64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal, TypeCode.DBNull
                    Dim resultvalue As Object = 0
                    If Compiler.TypeResolution.CheckNumericRange(originalValue, resultvalue, ExpressionType) Then
                        Return resultvalue
                    Else
                        Compiler.Report.ShowMessage(Messages.VBNC30439, ExpressionType.ToString)
                        Return New SByte
                    End If
                Case Else
                    Compiler.Report.ShowMessage(Messages.VBNC30060, originalValue.ToString, ExpressionType.ToString)
                    Return CSByte(0)
            End Select
        End Get
    End Property

    Overrides ReadOnly Property ExpressionType() As Mono.Cecil.TypeReference
        Get
            Return Compiler.TypeCache.System_SByte
        End Get
    End Property
End Class