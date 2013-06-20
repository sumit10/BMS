using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Printing;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace test
{
    /// <summary>
    /// Interaction logic for printtest.xaml
    /// </summary>
    public partial class printtest : Window
    {
        public printtest()
        {
            InitializeComponent();
        }

        private void PrintSimpleTextButton_Click(object sender, RoutedEventArgs e)
        {
        //    PrintDialog printDlg = new PrintDialog();

        //    // Create a FlowDocument dynamically.
        //    FlowDocument doc = CreateFlowDocument();
        //    doc.Name = "FlowDoc";

        //    // Create IDocumentPaginatorSource from FlowDocument
        //    IDocumentPaginatorSource idpSource = doc;

        //    // Call PrintDocument method to send document to printer
        //    printDlg.PrintDocument(idpSource.DocumentPaginator, "Hello WPF Printing.");    
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() != true) return;
            FixedDocument document = new FixedDocument();
            document.DocumentPaginator.PageSize = new Size(8.5,11);
            FixedPage page1 = new FixedPage();
            page1.Width = document.DocumentPaginator.PageSize.Width;
            page1.Height = document.DocumentPaginator.PageSize.Height;
            Canvas can = new Canvas();
            TextBlock page1Text = new TextBlock();
            page1Text.Text = "This is the first page";
            page1Text.FontSize = 10; // 30pt text
            page1Text.Margin = new Thickness(5, 20, 30, 10);
            can.Children.Add(page1Text);
             // 1 inch margin
            //page1.Children.Add(page1Text);
            page1.Width = document.DocumentPaginator.PageSize.Width;
            page1.Height = document.DocumentPaginator.PageSize.Height;
            TextBlock page2Text = new TextBlock();
            page2Text.Text = "This is the first page";
            page2Text.FontSize = 40; // 30pt text
            page2Text.Margin = new Thickness(5, 20, 30, 10); // 1 inch margin
            can.Children.Add(page2Text);
            page1.Children.Add(can);
            PageContent page1Content = new PageContent();
            ((IAddChild)page1Content).AddChild(page1);
            document.Pages.Add(page1Content);
            //FixedPage page2 = new FixedPage();
            //page2.Width = document.DocumentPaginator.PageSize.Width;
            //page2.Height = document.DocumentPaginator.PageSize.Height;
            //TextBlock page2Text = new TextBlock();
            //page2Text.Text = "This is NOT the first page";
            //page2Text.FontSize = 40;
            //page2Text.Margin = new Thickness(96);
            //page2.Children.Add(page2Text);
            //PageContent page2Content = new PageContent();
            //((IAddChild)page2Content).AddChild(page2);
            //document.Pages.Add(page2Content);
            pd.PrintDocument(document.DocumentPaginator, "My first document");
        }

        //private FlowDocument CreateFlowDocument()
        //{
        //    // Create a FlowDocument
        //    FlowDocument doc = new FlowDocument();

        //    // Create a Section
        //    Section sec = new Section();

        //    // Create first Paragraph
        //    Paragraph p1 = new Paragraph();
        //    // Create and add a new Bold, Italic and Underline
        //    Bold bld = new Bold();
        //    bld.Inlines.Add(new Run("First Paragraph"));
        //    Italic italicBld = new Italic();
        //    italicBld.Inlines.Add(bld);
        //    Underline underlineItalicBld = new Underline();
        //    underlineItalicBld.Inlines.Add(italicBld);
        //    // Add Bold, Italic, Underline to Paragraph
        //    p1.Inlines.Add(underlineItalicBld);

        //    // Add Paragraph to Section
        //    sec.Blocks.Add(p1);

        //    // Add Section to FlowDocument
        //    doc.Blocks.Add(sec);

        //    return doc;
        //}

    }
}
