using function;
using function.math;
using function.tree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace mlgui
{

    public partial class MLGUI : Form
    {
        public static FunctionTree ans0;
        public static FunctionTree ans;

        public static List<FunctionEntry> ML_Functions;
        public static List<VariableEntry> ML_Variables;

        public static int AddItem = 0;
        public static int RemoveItem = 1;
        public static int RemoveLastItem = 2;
        public static int UpdateVariableValue = 4;
        public static int UpdateFunctionExpression = 5;
        public static int GetVariable = 6;
        public static int GetFunction = 7;
        public static int GetFunctionFromExpression = 7;

        public static FunctionEntry lastFunctionEntry;
        public static VariableEntry lastVariableEntry;

        public static int liveEditorTabCount = 1;
        public static int lastProcessedLineNo = 0;
        public static int lastEnteredTextIndex = 0;

        private static bool dgv1_selChanged = false;
        private static bool dgv2_selChanged = false;
        public static int timer1_ms = 0;
        public static bool processing = false;

        public MLGUI()
        {
            InitializeComponent();

            liveEditorWin.Padding = new Point(12, 4);
            liveEditorWin.DrawMode = TabDrawMode.OwnerDrawFixed;

            liveEditorWin.DrawItem += liveEditorWin_DrawItem;
            liveEditorWin.MouseDown += liveEditorWin_MouseDown;
            liveEditorWin.Selecting += liveEditorWin_Selecting;
            liveEditorWin.HandleCreated += liveEditorWin_HandleCreated;

            CommParser.r2 = r2;
            CommParser.r3 = r3;

            CommParser.mlgui = this;
            ListHandler.mlgui = this;
            ListHandler.r3 = this.r3;
            ML_Functions = new List<FunctionEntry>();
            ML_Variables = new List<VariableEntry>();
        }

        bool KP_Enter = false,
             KP_Backspace = false,
             KP_F1 = false;


        public int getWidth()
        {
            int w = 25;  
            int line = r2.Lines.Length;

            if (line <= 99)         w = 20 + (int)r2.Font.Size;
            else if (line <= 999)   w = 30 + (int)r2.Font.Size;
            else                    w = 50 + (int)r2.Font.Size;

            return w;
        }

        public void AddLineNumbers(int p1)
        { 
            Point pt = new Point(0, 0);

            int First_Index = r2.GetCharIndexFromPosition(pt);
            int First_Line = r2.GetLineFromCharIndex(First_Index);

            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;

            int Last_Index = r2.GetCharIndexFromPosition(pt);
            int Last_Line = r2.GetLineFromCharIndex(Last_Index);

            r1.SelectionAlignment = HorizontalAlignment.Center;
            r1.Text = "";
            r1.Width = getWidth();

            for (int i = First_Line; i <= Last_Line + p1; i++)      r1.Text += i + 1 + "\n";
        }

        public void UpdateVarWindow()
        {
            dataGridView1.Rows.Clear();
            foreach (VariableEntry x in ML_Variables)
            {
                dataGridView1.Rows.Add(x.Name, x.ToStringValue());
            }
        }

        public void UpdateFuncWindow()
        {
            dataGridView2.Rows.Clear();
            foreach (FunctionEntry x in ML_Functions)
            {
                dataGridView2.Rows.Add(x.Name, x.Function.ToString());
            }
        }

        private void r2_TextChanged(object sender, EventArgs e)
        {
            SuspendLayout();
            if (r2.Lines.Length != 0 && r2.Lines[r2.Lines.Length - 1] == "")
            {
                AddLineNumbers(1);
            }
            else AddLineNumbers(0);
            ResumeLayout();

            if (r2.Text == "")
            {
                Unreg_KeyPress();
                return;
            }

            string c = "";
            int i = r2.SelectionStart;

            if (i > 0)
            {
                c = r2.Text.Substring(i - 1, 1);
            }

            if (c != "\n" && c != " ") lastEnteredTextIndex = i-1;

            if (r2.SelectionStart < r2.Text.Length) lastEnteredTextIndex++;

            if (r2.Text != "" && KP_Backspace && r2.SelectionStart == lastEnteredTextIndex) lastEnteredTextIndex--;

            //r3.Text += c+" "+lastEnteredTextIndex +" "+ r2.Text[lastEnteredTextIndex]+ "\n";

            if (c == "\n" && KP_Backspace==false && r2.SelectionStart > lastEnteredTextIndex && !CommParser.processing)
            {
                if (r2.Lines[r2.Lines.Length-2] == "")
                {
                    Unreg_KeyPress();
                    return;
                }

                processing = true;

                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 1;
                timer1_ms = 0;
                timer1.Start();

                SuspendLayout();

                int end = r2.Text.Length;

                while(r2.Text[end-1] == ' ')
                {
                    end--;
                }


                r2.Select(0, end);

                r2.SelectionProtected = true;
                r2.SelectionStart = r2.TextLength;
                r2.ScrollToCaret();

                ResumeLayout();
                                           
                               
                CommParser.ProcessInput(r2.Lines.Length-2); // Process Last Input Line
                lastProcessedLineNo = r2.Lines.Length - 2;

                UpdateVarWindow();
                UpdateFuncWindow();

                dataGridView1.ClearSelection();
                dataGridView2.ClearSelection();

                SuspendLayout();
                r3.SelectionStart = r3.TextLength;
                r3.ScrollToCaret();
                ResumeLayout();
                                
                processing = false;

                if (timer1_ms >= 20)
                {
                    progressBar1.MarqueeAnimationSpeed = 0;
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = 0;
                }
            }
            Unreg_KeyPress();
        }



        public void MLGUI_Load(object sender, EventArgs e)
        {
            r1.Font = r2.Font;
            r2.Select();
            AddLineNumbers(0);
            Activate();
        }

        private void r2_VScroll(object sender, EventArgs e)
        {
            r1.Text = "";
            AddLineNumbers(0);
            r1.Invalidate();

        }

        private void r2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void r2_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void MLGUI_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void MLGUI_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Unreg_KeyPress()
        {
            KP_Enter = false;
            KP_Backspace = false;
            KP_F1 = false;
        }


        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void MLGUI_Resize(object sender, EventArgs e)
        {
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void r2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) KP_Enter = true;
            if (e.KeyCode == Keys.Back) KP_Backspace = true;
            if (e.KeyCode == Keys.F1) KP_F1 = true;

            // History Scroll
            if (e.KeyCode == Keys.Up)
                r2.GetLineFromCharIndex(r2.SelectionStart);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void maskedTextBox2_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void modeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void consoleEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!consoleEntryToolStripMenuItem.Checked)
            {
                liveEditorToolStripMenuItem.Checked = false;
                consoleEntryToolStripMenuItem.Checked = true;

                r2.Visible = true;
                liveEditorWin.Visible = false;

                r1.Visible = true;
                r1_live.Visible = false;

                panel3.Visible = true;
            }
        }

        private void liveEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!liveEditorToolStripMenuItem.Checked)
            {
                consoleEntryToolStripMenuItem.Checked = false;
                liveEditorToolStripMenuItem.Checked = true;

                r2.Visible = false;
                liveEditorWin.Visible = true;

                r1.Visible = false;
                r1_live.Visible = true;

                panel3.Visible = false;
            }
        }

        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
          
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;

        private void liveEditorWin_HandleCreated(object sender, EventArgs e)
        {
            SendMessage(liveEditorWin.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
        }

        private void liveEditorWin_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == liveEditorWin.TabCount - 1)
                e.Cancel = true;
        }

        private void dataStructureToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            Close();
        }


        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!lightToolStripMenuItem.Checked)
            {
                darkToolStripMenuItem.Checked = false;
                lightToolStripMenuItem.Checked = true;


            }
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!darkToolStripMenuItem.Checked)
            {
                lightToolStripMenuItem.Checked = false;
                darkToolStripMenuItem.Checked = true;

                BackColor = Color.FromArgb(45, 45, 48);
                r2.BackColor = Color.FromArgb(30, 30, 30);
                r2.ForeColor = Color.FromArgb(255, 255, 255);
                panel3.BackColor = Color.FromArgb(51, 51, 55);
                r1.Enabled = true;
                r1.BackColor = Color.FromArgb(45, 45, 48);
                r1.ForeColor = Color.FromArgb(43, 145, 175);
                menuStrip1.BackColor = Color.FromArgb(45, 45, 48);
                menuStrip1.ForeColor = Color.FromArgb(255, 255, 255);
                modeToolStripMenuItem.BackColor = Color.FromArgb(45, 45, 48);
                modeToolStripMenuItem.DropDown.BackColor = Color.FromArgb(45, 45, 48);
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!dgv2_selChanged)
            {
                dataGridView2.ClearSelection();
                dgv2_selChanged = true;
            }
            else
            {
                dgv2_selChanged = false;
            }

            if (dataGridView2.SelectedRows.Count != 0)
            {
                funcTreeText.Text = "\n" + ML_Functions[dataGridView2.SelectedRows[0].Index].Function.GetFunctionTreeText();
            }
            else funcTreeText.Text = "";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!dgv1_selChanged)
            {
                dataGridView1.ClearSelection();
                dgv1_selChanged = true;
            }
            else
            {
                dgv1_selChanged = false;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dgv1_selChanged = true;
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            dgv2_selChanged = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1_ms++;

            if (timer1_ms>=20)
            {
                timer1.Stop();
                if (!processing)
                {
                    progressBar1.MarqueeAnimationSpeed = 0;
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = 0;
                }
            }
        }

        private void liveEditorWin_MouseDown(object sender, MouseEventArgs e)
        {
            var lastIndex = liveEditorWin.TabCount - 1;
            if (liveEditorWin.GetTabRect(lastIndex).Contains(e.Location))
            {
                if(liveEditorTabCount==0)
                    liveEditorWin.TabPages.Insert(lastIndex, "Untitled.txt*");
                else liveEditorWin.TabPages.Insert(lastIndex, "Untitled"+liveEditorTabCount+".txt*");
                liveEditorWin.SelectedIndex = lastIndex;
                liveEditorTabCount++;
            }
            else
            {
                for (var i = 0; i < liveEditorWin.TabPages.Count; i++)
                {
                    var tabRect = liveEditorWin.GetTabRect(i);
                    tabRect.Inflate(-2, -2);
                    var closeImage = MathLib.Properties.Resources.Del1;
                    var imageRect = new Rectangle(
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                        closeImage.Width,
                        closeImage.Height);
                    if (imageRect.Contains(e.Location))
                    {
                        liveEditorWin.TabPages.RemoveAt(i);
                        if (liveEditorWin.TabPages.Count == 1) liveEditorTabCount = 0;
                        break;
                    }
                }
            }
        }

        private void liveEditorWin_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = liveEditorWin.TabPages[e.Index];
            var tabRect = liveEditorWin.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);
            if (e.Index == liveEditorWin.TabCount - 1)
            {
                var addImage = MathLib.Properties.Resources.Add1;
                e.Graphics.DrawImage(addImage,
                    tabRect.Left + (tabRect.Width - addImage.Width) / 2,
                    tabRect.Top + (tabRect.Height - addImage.Height) / 2);
            }
            else
            {
                var closeImage = MathLib.Properties.Resources.Del1;
                e.Graphics.DrawImage(closeImage,
                    (tabRect.Right - closeImage.Width),
                    tabRect.Top + (tabRect.Height - closeImage.Height) / 2);
                TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                    tabRect, tabPage.ForeColor, TextFormatFlags.Left);
            }
        }


    }

}