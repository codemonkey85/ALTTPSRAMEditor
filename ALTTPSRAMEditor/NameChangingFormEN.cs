// ReSharper disable LocalizableElement

using static ALTTPSRAMEditor.Properties.Resources;

namespace ALTTPSRAMEditor;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility",
    Justification = "This is a Windows Forms application.")]
[SuppressMessage("Style", "IDE1006:Naming Styles")]
public partial class NameChangingFormEn : Form
{
    private readonly StringBuilder currName;
    private readonly ushort[] currNameRaw;
    private readonly Dictionary<char, int> enChar;
    private readonly MainForm mainForm;
    private readonly Dictionary<ushort, char> rawEnChar;
    private bool autoClose;
    private int charPos;
    private Bitmap enFnt = null!;

    public NameChangingFormEn(MainForm mainForm)
    {
        InitializeComponent();
        this.mainForm = mainForm;
        enChar = mainForm.TextCharacterData.EnChar;
        rawEnChar = mainForm.TextCharacterData.RawEnChar;
        autoClose = false;
        currName = new StringBuilder(this.mainForm.GetPlayerName()[..6]);
        currNameRaw = new ushort[6];
    }

    private void NameChangingFormEn_KeyDown(object sender, KeyEventArgs e)
    {
        // Close the Name Changing form if we hit Escape
        if (e.KeyCode == Keys.Escape)
        {
            Close();
        }
    }

    private void NameChangingFormEn_Load(object sender, EventArgs e)
    {
        // Name Changing Form Initialization
        enFnt = new Bitmap(en_font);

        // Draw the name to the screen
        UpdateDisplayName();

        kbdENCharA.Image = GetCharTexture(enFnt, 01, SaveRegion.USA, scale: 2);
        kbdENCharB.Image = GetCharTexture(enFnt, 02, SaveRegion.USA, scale: 2);
        kbdENCharC.Image = GetCharTexture(enFnt, 03, SaveRegion.USA, scale: 2);
        kbdENCharD.Image = GetCharTexture(enFnt, 04, SaveRegion.USA, scale: 2);
        kbdENCharE.Image = GetCharTexture(enFnt, 05, SaveRegion.USA, scale: 2);
        kbdENCharF.Image = GetCharTexture(enFnt, 06, SaveRegion.USA, scale: 2);
        kbdENCharG.Image = GetCharTexture(enFnt, 07, SaveRegion.USA, scale: 2);
        kbdENCharH.Image = GetCharTexture(enFnt, 08, SaveRegion.USA, scale: 2);
        kbdENCharI.Image = GetCharTexture(enFnt, 09, SaveRegion.USA, scale: 2);
        kbdENCharJ.Image = GetCharTexture(enFnt, 10, SaveRegion.USA, scale: 2);
        kbdENCharK.Image = GetCharTexture(enFnt, 11, SaveRegion.USA, scale: 2);
        kbdENCharL.Image = GetCharTexture(enFnt, 12, SaveRegion.USA, scale: 2);
        kbdENCharM.Image = GetCharTexture(enFnt, 13, SaveRegion.USA, scale: 2);
        kbdENCharN.Image = GetCharTexture(enFnt, 14, SaveRegion.USA, scale: 2);
        kbdENCharO.Image = GetCharTexture(enFnt, 15, SaveRegion.USA, scale: 2);
        kbdENCharP.Image = GetCharTexture(enFnt, 16, SaveRegion.USA, scale: 2);
        kbdENCharQ.Image = GetCharTexture(enFnt, 17, SaveRegion.USA, scale: 2);
        kbdENCharR.Image = GetCharTexture(enFnt, 18, SaveRegion.USA, scale: 2);
        kbdENCharS.Image = GetCharTexture(enFnt, 19, SaveRegion.USA, scale: 2);
        kbdENCharT.Image = GetCharTexture(enFnt, 20, SaveRegion.USA, scale: 2);
        kbdENCharU.Image = GetCharTexture(enFnt, 21, SaveRegion.USA, scale: 2);
        kbdENCharV.Image = GetCharTexture(enFnt, 22, SaveRegion.USA, scale: 2);
        kbdENCharW.Image = GetCharTexture(enFnt, 23, SaveRegion.USA, scale: 2);
        kbdENCharX.Image = GetCharTexture(enFnt, 24, SaveRegion.USA, scale: 2);
        kbdENCharY.Image = GetCharTexture(enFnt, 25, SaveRegion.USA, scale: 2);
        kbdENCharZ.Image = GetCharTexture(enFnt, 26, SaveRegion.USA, scale: 2);

        kbdENCharSmallA.Image = GetCharTexture(enFnt, 28, SaveRegion.USA, scale: 2);
        kbdENCharSmallB.Image = GetCharTexture(enFnt, 29, SaveRegion.USA, scale: 2);
        kbdENCharSmallC.Image = GetCharTexture(enFnt, 30, SaveRegion.USA, scale: 2);
        kbdENCharSmallD.Image = GetCharTexture(enFnt, 31, SaveRegion.USA, scale: 2);
        kbdENCharSmallE.Image = GetCharTexture(enFnt, 32, SaveRegion.USA, scale: 2);
        kbdENCharSmallF.Image = GetCharTexture(enFnt, 33, SaveRegion.USA, scale: 2);
        kbdENCharSmallG.Image = GetCharTexture(enFnt, 34, SaveRegion.USA, scale: 2);
        kbdENCharSmallH.Image = GetCharTexture(enFnt, 35, SaveRegion.USA, scale: 2);
        kbdENCharSmallI.Image = GetCharTexture(enFnt, 36, SaveRegion.USA, scale: 2);
        kbdENCharSmallJ.Image = GetCharTexture(enFnt, 37, SaveRegion.USA, scale: 2);
        kbdENCharSmallK.Image = GetCharTexture(enFnt, 38, SaveRegion.USA, scale: 2);
        kbdENCharSmallL.Image = GetCharTexture(enFnt, 39, SaveRegion.USA, scale: 2);
        kbdENCharSmallM.Image = GetCharTexture(enFnt, 40, SaveRegion.USA, scale: 2);
        kbdENCharSmallN.Image = GetCharTexture(enFnt, 41, SaveRegion.USA, scale: 2);
        kbdENCharSmallO.Image = GetCharTexture(enFnt, 42, SaveRegion.USA, scale: 2);
        kbdENCharSmallP.Image = GetCharTexture(enFnt, 43, SaveRegion.USA, scale: 2);
        kbdENCharSmallQ.Image = GetCharTexture(enFnt, 44, SaveRegion.USA, scale: 2);
        kbdENCharSmallR.Image = GetCharTexture(enFnt, 45, SaveRegion.USA, scale: 2);
        kbdENCharSmallS.Image = GetCharTexture(enFnt, 46, SaveRegion.USA, scale: 2);
        kbdENCharSmallT.Image = GetCharTexture(enFnt, 47, SaveRegion.USA, scale: 2);
        kbdENCharSmallU.Image = GetCharTexture(enFnt, 48, SaveRegion.USA, scale: 2);
        kbdENCharSmallV.Image = GetCharTexture(enFnt, 49, SaveRegion.USA, scale: 2);
        kbdENCharSmallW.Image = GetCharTexture(enFnt, 50, SaveRegion.USA, scale: 2);
        kbdENCharSmallX.Image = GetCharTexture(enFnt, 51, SaveRegion.USA, scale: 2);
        kbdENCharSmallY.Image = GetCharTexture(enFnt, 52, SaveRegion.USA, scale: 2);
        kbdENCharSmallZ.Image = GetCharTexture(enFnt, 53, SaveRegion.USA, scale: 2);

        kbdENCharHyphen.Image = GetCharTexture(enFnt, 54, SaveRegion.USA, scale: 2);
        kbdENCharPeriod.Image = GetCharTexture(enFnt, 55, SaveRegion.USA, scale: 2);
        kbdENCharComma.Image = GetCharTexture(enFnt, 56, SaveRegion.USA, scale: 2);

        kbdENChar0.Image = GetCharTexture(enFnt, 59, SaveRegion.USA, scale: 2);
        kbdENChar1.Image = GetCharTexture(enFnt, 60, SaveRegion.USA, scale: 2);
        kbdENChar2.Image = GetCharTexture(enFnt, 61, SaveRegion.USA, scale: 2);
        kbdENChar3.Image = GetCharTexture(enFnt, 62, SaveRegion.USA, scale: 2);
        kbdENChar4.Image = GetCharTexture(enFnt, 63, SaveRegion.USA, scale: 2);
        kbdENChar5.Image = GetCharTexture(enFnt, 64, SaveRegion.USA, scale: 2);
        kbdENChar6.Image = GetCharTexture(enFnt, 65, SaveRegion.USA, scale: 2);
        kbdENChar7.Image = GetCharTexture(enFnt, 66, SaveRegion.USA, scale: 2);
        kbdENChar8.Image = GetCharTexture(enFnt, 67, SaveRegion.USA, scale: 2);
        kbdENChar9.Image = GetCharTexture(enFnt, 68, SaveRegion.USA, scale: 2);
        kbdENCharExclamation.Image = GetCharTexture(enFnt, 69, SaveRegion.USA, scale: 2);
        kbdENCharQuestion.Image = GetCharTexture(enFnt, 70, SaveRegion.USA, scale: 2);
        kbdENCharParenthaseesLeft.Image = GetCharTexture(enFnt, 71, SaveRegion.USA, scale: 2);
        kbdENCharParenthaseesRight.Image = GetCharTexture(enFnt, 72, SaveRegion.USA, scale: 2);

        kbdENMoveLeft.Image = GetCharTexture(enFnt, 57, SaveRegion.USA, scale: 2);
        kbdENMoveRight.Image = GetCharTexture(enFnt, 58, SaveRegion.USA, scale: 2);
        Refresh();
    }

    private void UpdateDisplayName()
    {
        pictureENNameChar0.Image = GetCharTexture(enFnt, enChar[currName[0]], SaveRegion.USA, true);
        pictureENNameChar1.Image = GetCharTexture(enFnt, enChar[currName[1]], SaveRegion.USA, true);
        pictureENNameChar2.Image = GetCharTexture(enFnt, enChar[currName[2]], SaveRegion.USA, true);
        pictureENNameChar3.Image = GetCharTexture(enFnt, enChar[currName[3]], SaveRegion.USA, true);
        pictureENNameChar4.Image = GetCharTexture(enFnt, enChar[currName[4]], SaveRegion.USA, true);
        pictureENNameChar5.Image = GetCharTexture(enFnt, enChar[currName[5]], SaveRegion.USA, true);

        pictureENCharHeart.Location = new Point(62 + charPos * 32, 174);
    }

    private void TypeChar(char c)
    {
        currName[charPos] = c;

        charPos++;
        if (charPos > 5)
        {
            charPos = 0;
        }

        // Draw the name to the screen
        UpdateDisplayName();
    }

    private void UpdatePlayerName()
    {
        // Update MainForm with the changed player name
        // If the name is too short, fill it with spaces
        for (var k = currName.Length; k < 6; k++)
        {
            currName[k] = ' ';
        }

        mainForm.SetPlayerName(currName.ToString());
        for (var i = 0; i < currName.Length; i++)
        {
            currNameRaw[i] = rawEnChar.FirstOrDefault(x => x.Value == currName[i]).Key;
        }

        mainForm.SetPlayerNameRaw(currNameRaw);
        mainForm.UpdatePlayerName();
    }

    private void NameChangingFormEn_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (autoClose)
        {
            return;
        }

        var dialogSave = MessageBox.Show(
            "Would you like to save your changes?",
            "Save Changes?",
            MessageBoxButtons.YesNo);
        if (dialogSave == DialogResult.Yes)
        {
            UpdatePlayerName();
        }
        else
        {
            var dialogCloseConfirm = MessageBox.Show(
                "Continue editing?",
                "Closing Name Changing Form (USA/EUR)",
                MessageBoxButtons.YesNo);
            if (dialogCloseConfirm == DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }

    private void kbdEnMoveLeft_Click(object sender, EventArgs e)
    {
        charPos--;
        if (charPos < 0)
        {
            charPos = 5;
        }

        pictureENCharHeart.Location = new Point(62 + charPos * 32, 174);
    }

    private void kbdEnMoveRight_Click(object sender, EventArgs e)
    {
        charPos++;
        if (charPos > 5)
        {
            charPos = 0;
        }

        pictureENCharHeart.Location = new Point(62 + charPos * 32, 174);
    }

    private void KbdEnChar_Click(object sender, EventArgs e)
    {
        char? charToType = (sender as PictureBox)?.Name switch
        {
            nameof(kbdENCharA) => 'A',
            nameof(kbdENCharB) => 'B',
            nameof(kbdENCharC) => 'C',
            nameof(kbdENCharD) => 'D',
            nameof(kbdENCharE) => 'E',
            nameof(kbdENCharF) => 'F',
            nameof(kbdENCharG) => 'G',
            nameof(kbdENCharH) => 'H',
            nameof(kbdENCharI) => 'I',
            nameof(kbdENCharJ) => 'J',
            nameof(kbdENCharK) => 'K',
            nameof(kbdENCharL) => 'L',
            nameof(kbdENCharM) => 'M',
            nameof(kbdENCharN) => 'N',
            nameof(kbdENCharO) => 'O',
            nameof(kbdENCharP) => 'P',
            nameof(kbdENCharQ) => 'Q',
            nameof(kbdENCharR) => 'R',
            nameof(kbdENCharS) => 'S',
            nameof(kbdENCharT) => 'T',
            nameof(kbdENCharU) => 'U',
            nameof(kbdENCharV) => 'V',
            nameof(kbdENCharW) => 'W',
            nameof(kbdENCharX) => 'X',
            nameof(kbdENCharY) => 'Y',
            nameof(kbdENCharZ) => 'Z',

            nameof(kbdENCharSmallA) => 'a',
            nameof(kbdENCharSmallB) => 'b',
            nameof(kbdENCharSmallC) => 'c',
            nameof(kbdENCharSmallD) => 'd',
            nameof(kbdENCharSmallE) => 'e',
            nameof(kbdENCharSmallF) => 'f',
            nameof(kbdENCharSmallG) => 'g',
            nameof(kbdENCharSmallH) => 'h',
            nameof(kbdENCharSmallI) => 'i',
            nameof(kbdENCharSmallJ) => 'j',
            nameof(kbdENCharSmallK) => 'k',
            nameof(kbdENCharSmallL) => 'l',
            nameof(kbdENCharSmallM) => 'm',
            nameof(kbdENCharSmallN) => 'n',
            nameof(kbdENCharSmallO) => 'o',
            nameof(kbdENCharSmallP) => 'p',
            nameof(kbdENCharSmallQ) => 'q',
            nameof(kbdENCharSmallR) => 'r',
            nameof(kbdENCharSmallS) => 's',
            nameof(kbdENCharSmallT) => 't',
            nameof(kbdENCharSmallU) => 'u',
            nameof(kbdENCharSmallV) => 'v',
            nameof(kbdENCharSmallW) => 'w',
            nameof(kbdENCharSmallX) => 'x',
            nameof(kbdENCharSmallY) => 'y',
            nameof(kbdENCharSmallZ) => 'z',

            nameof(kbdENChar0) => '0',
            nameof(kbdENChar1) => '1',
            nameof(kbdENChar2) => '2',
            nameof(kbdENChar3) => '3',
            nameof(kbdENChar4) => '4',
            nameof(kbdENChar5) => '5',
            nameof(kbdENChar6) => '6',
            nameof(kbdENChar7) => '7',
            nameof(kbdENChar8) => '8',
            nameof(kbdENChar9) => '9',

            nameof(kbdENCharHyphen) => '-',
            nameof(kbdENCharPeriod) => '.',
            nameof(kbdENCharComma) => ',',
            nameof(kbdENCharExclamation) => '!',
            nameof(kbdENCharQuestion) => '?',
            nameof(kbdENCharParenthaseesLeft) => '(',
            nameof(kbdENCharParenthaseesRight) => ')',
            nameof(kbdENSpace) => ' ',

            _ => null
        };
        if (charToType is not null)
        {
            TypeChar(charToType.Value);
        }
    }

    private void kbdEnEnd_Click(object sender, EventArgs e)
    {
        UpdatePlayerName();
        autoClose = true;
        Close();
    }
}