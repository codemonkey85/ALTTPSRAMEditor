﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALTTPSRAMEditor
{
    public partial class NameChangingFormJP : Form
    {
        private Bitmap jp_fnt;
        private StringBuilder currName;
        private UInt16[] currNameRaw;
        private Dictionary<char, int> jpChar;
        private Dictionary<UInt16, char> rawJPChar;
        private int charPos = 0;
        private bool autoClose;
        Form1 form1;

        public NameChangingFormJP(Form1 _form1)
        {
            InitializeComponent();
            form1 = _form1;
            jpChar = form1.GetJPChar();
            rawJPChar = form1.GetRawJPChar();
            autoClose = false;
            currName = new StringBuilder(form1.GetPlayerName().Substring(0, 4));
            currNameRaw = new UInt16[6];
        }

        private void NameChangingFormJP_KeyDown(object sender, KeyEventArgs e)
        {
            // Close the Name Changing form if we hit Escape
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void NameChangingFormJP_Load(object sender, EventArgs e)
        {

            // Name Changing Form Initialization
            jp_fnt = new Bitmap(ALTTPSRAMEditor.Properties.Resources.jpn_font);

            // Draw the name to the screen
            UpdateDisplayName();

            kbdHiraganaCharA.Image = GetCharTexture(jp_fnt, 9, false);
            kbdHiraganaCharI.Image = GetCharTexture(jp_fnt, 10, false);
            kbdHiraganaCharU.Image = GetCharTexture(jp_fnt, 11, false);
            /*
             * 
            kbdENCharD.Image = GetCharTexture(en_fnt, 4, false);
*/
            Refresh();
        }

        private void UpdateDisplayName()
        {
            pictureJPNameChar0.Image = GetCharTexture(jp_fnt, jpChar[currName[0]], false);
            pictureJPNameChar1.Image = GetCharTexture(jp_fnt, jpChar[currName[1]], false);
            pictureJPNameChar2.Image = GetCharTexture(jp_fnt, jpChar[currName[2]], false);
            pictureJPNameChar3.Image = GetCharTexture(jp_fnt, jpChar[currName[3]], false);

            pictureJPCharHeart.Location = new Point(880 + (charPos * 32), 38);
        }

        private static Image GetCharTexture(Bitmap jp_fnt, int tileID, bool hugLeft)
        {
            int tileset_width = 20; // Japanese Font
            int tile_w = 8;
            int tile_h = 16;
            int x = (tileID % tileset_width) * tile_w;
            int y = (tileID / tileset_width) * tile_h;
            int width = 8;
            int height = 16;
            int scale = 2;
            Rectangle crop = new Rectangle(x, y, width * scale, height * scale);
            var tex = new Bitmap(crop.Width, crop.Height);

            using (var gr = Graphics.FromImage(tex))
            {
                gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                gr.DrawImage(jp_fnt, new Rectangle(0, 0, tex.Width * scale, tex.Height * scale), crop, GraphicsUnit.Pixel);
            }

            if (hugLeft)
                return tex;

            var bmp = new Bitmap(crop.Width * 2, crop.Height);

            using (var gr = Graphics.FromImage(bmp))
            {
                gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                gr.Clear(Color.Black);
                gr.DrawImage(tex, 8, 2);
            }
            return bmp;
        }

        private void TypeChar(char c)
        {
            currName[charPos] = c;

            charPos++;
            if (charPos > 3)
                charPos = 0;

            // Draw the name to the screen
            UpdateDisplayName();
        }

        private void UpdatePlayerName()
        {
            // Update Form1 with the changed player name
            // If the name is too short, fill it with spaces
            for (int k = currName.Length; k < 4; k++)
                currName[k] = ' ';

            form1.SetPlayerName(currName.ToString());
            int j = 0;
            for (int i = 0; i < currName.Length; i++)
            {
                currNameRaw[i] = rawJPChar.FirstOrDefault(x => x.Value == currName[i]).Key;
                j++;
            }
            form1.SetPlayerNameRaw(currNameRaw);
            form1.UpdatePlayerName();
        }

        private void NameChangingFormJP_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (!autoClose)
            {
                DialogResult dialogSave = MessageBox.Show("Would you like to save your changes?", "Save Changes?", MessageBoxButtons.YesNo);
                if (dialogSave == DialogResult.Yes)
                {
                    UpdatePlayerName();
                }
                else
                {
                    DialogResult dialogCloseConfirm = MessageBox.Show("Continue editing?", "Closing Name Changing Form (JPN)", MessageBoxButtons.YesNo);
                    if (dialogCloseConfirm == DialogResult.Yes)
                        e.Cancel = true;
                }
            }*/
        }

        private void kbdJPMoveLeft_Click(object sender, EventArgs e)
        {
            charPos--;
            if (charPos < 0)
                charPos = 3;
            pictureJPCharHeart.Location = new Point(880 + (charPos * 32), 38);
        }

        private void kbdJPMoveRight_Click(object sender, EventArgs e)
        {
            charPos++;
            if (charPos > 3)
                charPos = 0;
            pictureJPCharHeart.Location = new Point(880 + (charPos * 32), 38);
        }

        private void kbdJPEnd_Click(object sender, EventArgs e)
        {
            UpdatePlayerName();
            autoClose = true;
            Close();
        }

        private void kbdHiraganaCharA_Click(object sender, EventArgs e)
        {
            TypeChar('あ');
        }

        private void kbdHiraganaCharI_Click(object sender, EventArgs e)
        {
            TypeChar('あ');
        }

        private void kbdHiraganaCharU_Click(object sender, EventArgs e)
        {
            TypeChar('あ');
        }
    }
}
