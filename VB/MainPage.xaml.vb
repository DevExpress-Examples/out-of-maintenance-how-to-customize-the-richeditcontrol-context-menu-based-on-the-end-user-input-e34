Imports Microsoft.VisualBasic
Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports DevExpress.Xpf.Bars
Imports DevExpress.Xpf.RichEdit
Imports DevExpress.Xpf.RichEdit.Menu
Imports DevExpress.XtraRichEdit.API.Native
Imports DevExpress.XtraRichEdit.Utils

Namespace RichEditInputSpecificMenuSL
	Partial Public Class MainPage
		Inherits UserControl
		Private keyTextTable As String = "Table:"
		Private keyTextView As String = "View:"
		Private userCommand As String = String.Empty
		Private ReadOnly Property Document() As Document
			Get
				Return reEditor.Document
			End Get
		End Property

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub reEditor_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			reEditor.ApplyTemplate()
			reEditor.KeyCodeConverter.Focus()
		End Sub

		Private Sub RichEditControl_ContentChanged(ByVal sender As Object, ByVal e As EventArgs)
			Dim maxKeyLength As Integer = Math.Max(keyTextTable.Length, keyTextView.Length)
			Dim text As String = Document.GetText(Document.CreateRange(Math.Max(0, Document.CaretPosition.ToInt() - maxKeyLength), maxKeyLength))

			If text.EndsWith(keyTextTable) OrElse text.EndsWith(keyTextView) Then
				If text.EndsWith(keyTextTable) Then
					userCommand = keyTextTable
				ElseIf text.EndsWith(keyTextView) Then
					userCommand = keyTextView
				Else
					userCommand = String.Empty
				End If

				Dim rect As System.Drawing.Rectangle = reEditor.GetBoundsFromPosition(Document.CaretPosition)
				Dim localRect As System.Drawing.Rectangle = Units.DocumentsToPixels(rect, reEditor.DpiX, reEditor.DpiY)
				Dim localPoint As New System.Drawing.Point(localRect.Right, localRect.Bottom)

				Dim generalTransform As GeneralTransform = Me.TransformToVisual(reEditor)
				Dim point As Point = generalTransform.Transform(New Point(localPoint.X, localPoint.Y))
				reEditor.ShowMenu(point)
			End If
		End Sub

		Private Sub reEditor_PopupMenuShowing(ByVal sender As Object, ByVal e As PopupMenuShowingEventArgs)
			If (Not String.IsNullOrEmpty(userCommand)) Then
				e.Menu.ItemLinks.Add(New BarItemLinkSeparator())

				If userCommand = keyTextTable Then
					Dim customItem As New RichEditMenuItem()
					customItem.Content = "Insert Table"
					customItem.Command = RichEditUICommand.InsertTable
					customItem.CommandParameter = reEditor
					e.Menu.ItemLinks.Add(customItem)
				Else
					Dim customItem1 As New RichEditMenuItem()
					customItem1.Content = "View Print Layout"
					customItem1.Command = RichEditUICommand.ViewPrintLayout
					customItem1.CommandParameter = reEditor
					e.Menu.ItemLinks.Add(customItem1)

					Dim customItem2 As New RichEditMenuItem()
					customItem2.Content = "View Draft"
					customItem2.Command = RichEditUICommand.ViewDraft
					customItem2.CommandParameter = reEditor
					e.Menu.ItemLinks.Add(customItem2)

					Dim customItem3 As New RichEditMenuItem()
					customItem3.Content = "View Simple"
					customItem3.Command = RichEditUICommand.ViewSimple
					customItem3.CommandParameter = reEditor
					e.Menu.ItemLinks.Add(customItem3)
				End If

				userCommand = String.Empty
			End If
		End Sub
	End Class

	Public Class CustomRichEditControl
		Inherits RichEditControl
		Public Sub ShowMenu(ByVal point As Point)
			MyBase.Focus()
			MyBase.OnPopupMenu(point)
		End Sub
	End Class
End Namespace
