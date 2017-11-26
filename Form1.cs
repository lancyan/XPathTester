using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace XPathTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
           
        }

        bool isIgnoreWhite = false;

        private void Run()
        {
            string xpath = "";
            string source = "";
            string result = "";

            treeView1.Nodes.Clear();
            richTextBox3.Clear();

            isIgnoreWhite = IgnoreWhiteToolStripMenuItem.Checked;

            xpath = richTextBox1.Text.Trim();
            source = richTextBox2.Text.Trim();
            //result = richTextBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(xpath))
            {
                toolStripStatusMsg.Text = "xpath不能为空！！";
                return;
            }
            if (xpath.EndsWith("/"))
            {
                toolStripStatusMsg.Text = "xpath不能以/结尾！！";
                return;
            }
            if (string.IsNullOrWhiteSpace(source))
            {
                toolStripStatusMsg.Text = "匹配内容不能为空！！";
                return;
            }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(source);

            HtmlNode rootnode = doc.DocumentNode;
            try
            {
                bool isHtml = innerHTMLToolStripMenuItem.Checked;
                if (matchSingleToolStripMenuItem.Checked)
                {
                    DoSingle(rootnode, xpath, isHtml);
                }
                else
                {
                    DoMult(rootnode, xpath, isHtml);
                }
            }
            catch (Exception ex)
            {
                toolStripStatusMsg.Text = ex.Message;
            }
        }


        private void CreateTree(HtmlNode rootNode, TreeNode tn, bool flag)
        {
            if (rootNode.HasChildNodes)
            {
                foreach (var node in rootNode.ChildNodes)
                {
                    string tagName = node.Name;
                    string str = flag ? node.OuterHtml : node.InnerText;
                    if (isIgnoreWhite)
                    {
                        if (!(tagName == "#text" && string.IsNullOrWhiteSpace(node.InnerText.Trim())))
                        {
                            //loopNode(tagName, str, node, flag);
                            TreeNode newNode = new TreeNode(tagName);
                            if (tagName == "#text")
                            {
                                newNode.Text = newNode.Text + "  " + str.Trim();
                            }
                            newNode.Tag = node.StreamPosition + ":" + node.OuterHtml.Length;
                            tn.Nodes.Add(newNode);
                            CreateTree(node, newNode,flag);
                        }
                    }
                    else
                    {
                        //loopNode(tagName, str, child, flag);
                        TreeNode newNode = new TreeNode(tagName);
                        if (tagName == "#text")
                        {
                            newNode.Text = newNode.Text + "  " + str.Trim();
                        }
                        newNode.Tag = node.StreamPosition + ":" + node.OuterHtml.Length;
                        tn.Nodes.Add(newNode);
                        CreateTree(node, newNode,flag);
                    }
                }
            }
        }

        private void DoSingle(HtmlNode rootNode, string xPath, bool flag)
        {
            HtmlNode node = rootNode.SelectSingleNode(xPath);
            if (node != null)
            {
                string str = flag ? node.OuterHtml : node.InnerText;
                richTextBox3.AppendText(str + "\r\n");
                if (flag)
                {
                    string tagName = node.Name;
                    if (isIgnoreWhite)
                    {
                        if (!(tagName == "#text" && string.IsNullOrWhiteSpace(str.Trim())))
                        {
                            loopNode(tagName, str, node,flag);
                        }
                    }
                    else
                    {
                        loopNode(tagName, str, node, flag);
                    }
                }
            }
        }
        private void DoMult(HtmlNode rootNode, string xPath, bool flag)
        {
            HtmlNodeCollection nodes = rootNode.SelectNodes(xPath);
            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    string str = flag ? node.OuterHtml : node.InnerText;
                    richTextBox3.AppendText(str + "\r\n");

                    string tagName = node.Name;
                    if (isIgnoreWhite)
                    {
                        if (!(tagName == "#text" && string.IsNullOrWhiteSpace(str.Trim())))
                        {
                            loopNode(tagName, str, node, flag);
                        }
                    }
                    else
                    {
                        loopNode(tagName, str, node, flag);
                    }

                }
            }

        }
        private void loopNode(string tagName, string str, HtmlNode node, bool flag)
        {
            TreeNode newNode = new TreeNode(tagName);
            if (tagName == "#text")
            {
                newNode.Text = newNode.Text + "  " + str.Trim();
            }
            newNode.Tag = node.StreamPosition + ":" + node.OuterHtml.Length;
            treeView1.Nodes.Add(newNode);
            CreateTree(node, newNode, flag);
        }

     

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Run();
            }
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void 树形结构ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Visible = true;
            richTextBox3.Visible = false;
        }

        private void 文本结构ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Visible = false;
            richTextBox3.Visible = true;
        }

  

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem mi in toolStripDropDownButton1.DropDownItems)
            {
                mi.Checked = false;
            }
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
        }

        private void toolStripDropDownButton2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            bool isChecked = ((ToolStripMenuItem)e.ClickedItem).Checked;
            ((ToolStripMenuItem)e.ClickedItem).Checked = !isChecked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void ClearSelection(RichTextBox rtb)
        {
            if (rtb.Text.Length > 0)
            {
                int currentIndex = rtb.SelectionStart;
                rtb.SelectAll();
                rtb.SelectionBackColor = Color.White;
                rtb.SelectionColor = SystemColors.WindowText;
                rtb.SelectionLength = 0;
                rtb.SelectionStart = currentIndex;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Tag != null)
            {
                int[] arr = Array.ConvertAll(node.Tag.ToString().Split(':'), int.Parse);
                ClearSelection(richTextBox2);
                richTextBox2.Select(arr[0], arr[1]);
                richTextBox2.SelectionColor = Color.White;
                richTextBox2.SelectionBackColor = Color.Red;
                richTextBox2.ScrollToCaret();
            }
        }
    }
}
