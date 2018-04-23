using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Menu;
using DevExpress.XtraRichEdit.Utils;

namespace RichEditInputSpecificMenuSL {
    public partial class MainPage : UserControl {
        private string keyTextTable = "Table:";
        private string keyTextView = "View:";
        private string userCommand = string.Empty;
        private Document Document { get { return reEditor.Document; } }
        
        public MainPage() {
            InitializeComponent();
        }

        private void reEditor_Loaded(object sender, RoutedEventArgs e) {
            reEditor.ApplyTemplate();
            reEditor.KeyCodeConverter.Focus();
        }

        private void RichEditControl_ContentChanged(object sender, EventArgs e) {
            int maxKeyLength = Math.Max(keyTextTable.Length, keyTextView.Length);
            string text = Document.GetText(Document.CreateRange(Math.Max(0, Document.CaretPosition.ToInt() - maxKeyLength), maxKeyLength));

            if (text.EndsWith(keyTextTable) || text.EndsWith(keyTextView)) {
                if (text.EndsWith(keyTextTable))
                    userCommand = keyTextTable;
                else if (text.EndsWith(keyTextView))
                    userCommand = keyTextView;
                else
                    userCommand = string.Empty;
                
                System.Drawing.Rectangle rect = reEditor.GetBoundsFromPosition(Document.CaretPosition);
                System.Drawing.Rectangle localRect = Units.DocumentsToPixels(rect, reEditor.DpiX, reEditor.DpiY);
                System.Drawing.Point localPoint = new System.Drawing.Point(localRect.Right, localRect.Bottom);

                GeneralTransform generalTransform = this.TransformToVisual(reEditor);
                Point point = generalTransform.Transform(new Point(localPoint.X, localPoint.Y));
                reEditor.ShowMenu(point);
            }
        }

        private void reEditor_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
            if (!string.IsNullOrEmpty(userCommand)) {
                e.Menu.ItemLinks.Add(new BarItemLinkSeparator());

                if (userCommand == keyTextTable) {
                    RichEditMenuItem customItem = new RichEditMenuItem();
                    customItem.Content = "Insert Table";
                    customItem.Command = RichEditUICommand.InsertTable;
                    customItem.CommandParameter = reEditor;
                    e.Menu.ItemLinks.Add(customItem);
                }
                else {
                    RichEditMenuItem customItem1 = new RichEditMenuItem();
                    customItem1.Content = "View Print Layout";
                    customItem1.Command = RichEditUICommand.ViewPrintLayout;
                    customItem1.CommandParameter = reEditor;
                    e.Menu.ItemLinks.Add(customItem1);

                    RichEditMenuItem customItem2 = new RichEditMenuItem();
                    customItem2.Content = "View Draft";
                    customItem2.Command = RichEditUICommand.ViewDraft;
                    customItem2.CommandParameter = reEditor;
                    e.Menu.ItemLinks.Add(customItem2);

                    RichEditMenuItem customItem3 = new RichEditMenuItem();
                    customItem3.Content = "View Simple";
                    customItem3.Command = RichEditUICommand.ViewSimple;
                    customItem3.CommandParameter = reEditor;
                    e.Menu.ItemLinks.Add(customItem3);
                }

                userCommand = string.Empty;
            }
        }
    }

    public class CustomRichEditControl : RichEditControl {
        public void ShowMenu(Point point) {
            base.Focus();
            base.OnPopupMenu(point);
        }
    }
}
