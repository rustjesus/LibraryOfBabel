using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace LibraryOfBabel
{
    public partial class Form1 : Form
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ,.";
        private const int PageLength = 3200;

        public Form1()
        {
            InitializeComponent();
            rtbOutput.ReadOnly = true;
            //rtbOutput.Cursor = Cursors.Default;
            //rtbOutput.ShortcutsEnabled = false;
            //rtbOutput.TabStop = false;
            rtbOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblVolume.Text = "Volume: 1";
            lblShelf.Text = "Shelf: 1";
            lblWall.Text = "Wall: 1";
            pageRtb.Text = "1";
            string hex = "00000000000000000000";
            rtbHex.Text = hex;
            GoToPage(hex, 1, 1, 1, 1);

        }
        private void GoToPage(string hex, int wall, int shelf, int volume, int page)
        {
            // Generate page text
            string pageText = GeneratePage(hex, wall, shelf, volume, page);

            // Display nicely
            rtbOutput.Text =
                $"Location:\n" +
                $"Hex: {hex}\n" +
                $"Wall: {wall}\n" +
                $"Shelf: {shelf}\n" +
                $"Volume: {volume}\n" +
                $"Page: {page}\n\n" +
                $"=== PAGE TEXT ===\n" +
                $"{pageText}";

            // Reset highlighting
            rtbOutput.Select(0, 0);
            rtbOutput.SelectionColor = System.Drawing.Color.Black;
        }

        // ------------------------------
        // Library of Babel – Page Logic
        // ------------------------------

        private string GeneratePage(string hex, int wall, int shelf, int volume, int page)
        {
            string seed = $"{hex}-{wall}-{shelf}-{volume}-{page}";

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(seed));
                int intSeed = BitConverter.ToInt32(hash, 0);

                Random rng = new Random(intSeed);

                StringBuilder builder = new StringBuilder(PageLength);

                for (int i = 0; i < PageLength; i++)
                {
                    builder.Append(Alphabet[rng.Next(Alphabet.Length)]);
                }

                return builder.ToString();
            }
        }

        private (string hex, int wall, int shelf, int volume, int page) RandomLocation()
        {
            Random r = new Random();

            string hex = "";
            for (int i = 0; i < 20; i++)
            {
                hex += "0123456789abcdef"[r.Next(16)];
            }

            int wall = r.Next(1, 5);
            int shelf = r.Next(1, 6);
            int volume = r.Next(1, 33);
            int page = r.Next(1, 411);

            return (hex, wall, shelf, volume, page);
        }

        // ------------------------------
        // BUTTON: SEARCH
        // ------------------------------

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }
        private string GeneratePageWithPhrase(
    string hex, int wall, int shelf, int volume, int page,
    string phrase, int insertIndex)
        {
            string pageText = GeneratePage(hex, wall, shelf, volume, page);

            StringBuilder sb = new StringBuilder(pageText);
            sb.Remove(insertIndex, phrase.Length);
            sb.Insert(insertIndex, phrase);

            return sb.ToString();
        }
        private (string hex, int wall, int shelf, int volume, int page, int insertIndex)
        LocationFromPhrase(string phrase)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(phrase));

                string hex = BitConverter.ToString(hash).Replace("-", "").Substring(0, 20).ToLower();

                int wall = (hash[20] % 4) + 1;
                int shelf = (hash[21] % 5) + 1;
                int volume = (hash[22] % 32) + 1;
                int page = (hash[23] % 410) + 1;

                int insertIndex = hash[24] % (PageLength - phrase.Length); // keep for internal use

                return (hex, wall, shelf, volume, page, insertIndex);
            }
        }


        // Class-level variable to store last searched phrase and location
        private string lastSearchedPhrase = null;
        private (string hex, int wall, int shelf, int volume, int page, int insertIndex)? lastPhraseLocation = null;

        // Update button1_Click
        private void button1_Click(object sender, EventArgs e)
        {
            string phrase = txtSearch.Text.Trim();
            if (phrase.Length == 0)
            {
                MessageBox.Show("Enter a phrase to search.");
                return;
            }

            var loc = LocationFromPhrase(phrase);

            string pageText = GeneratePageWithPhrase(
                loc.hex, loc.wall, loc.shelf, loc.volume, loc.page,
                phrase, loc.insertIndex);

            rtbOutput.Text =
                $"Your phrase exists at:\n" +
                $"Hex: {loc.hex}\n" +
                $"Wall: {loc.wall}\n" +
                $"Shelf: {loc.shelf}\n" +
                $"Volume: {loc.volume}\n" +
                $"Page: {loc.page}\n\n" +
                $"=== PAGE TEXT ===\n" +
                $"{pageText}";


            lblVolume.Text = "Volume: " + loc.volume.ToString();
            tbVolume.Value = loc.volume;
            lblShelf.Text = "Shelf: " + loc.shelf.ToString();
            tbShelf.Value = loc.shelf;
            lblWall.Text = "Wall: " + loc.wall.ToString();
            tbWall.Value = loc.wall;
            pageRtb.Text = loc.page.ToString();
            rtbHex.Text = loc.hex;


            // Highlight
            int index = rtbOutput.Text.IndexOf(phrase, StringComparison.Ordinal);
            if (index >= 0)
            {
                rtbOutput.Select(index, phrase.Length);
                rtbOutput.SelectionColor = System.Drawing.Color.Red;
                rtbOutput.SelectionBackColor = System.Drawing.Color.Transparent;
                rtbOutput.Select(0, 0);
            }

            // Save phrase and location for later use
            lastSearchedPhrase = phrase;
            lastPhraseLocation = loc;
        }


        private void hex_richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void findVolButton2_Click(object sender, EventArgs e)
        {
            string hex = rtbHex.Text.Trim();
            int wall = tbWall.Value;
            int shelf = tbShelf.Value;
            int volume = tbVolume.Value;
            if (!int.TryParse(pageRtb.Text.Trim(), out int page))
            {
                MessageBox.Show("Page must be a number.");
                return;
            }
            if (page < 1 || page > 410)
            {
                MessageBox.Show("Page number must be between 1 and 410.");
                return;
            }

            // Check if this location matches last searched phrase
            if (lastPhraseLocation.HasValue)
            {
                var loc = lastPhraseLocation.Value;
                if (loc.hex == hex && loc.wall == wall && loc.shelf == shelf && loc.volume == volume && loc.page == page)
                {
                    string pageText = GeneratePageWithPhrase(loc.hex, loc.wall, loc.shelf, loc.volume, loc.page, lastSearchedPhrase, loc.insertIndex);
                    rtbOutput.Text =
                        $"Location:\nHex: {hex}\nWall: {wall}\nShelf: {shelf}\nVolume: {volume}\nPage: {page}\n\n=== PAGE TEXT ===\n{pageText}";
                    return;
                }
            }

            // Normal page if no phrase at this location
            GoToPage(hex, wall, shelf, volume, page);
        }

        private void vol_trackBar3_Scroll(object sender, EventArgs e)
        {

            lblVolume.Text = $"Volume: {tbVolume.Value}";
        }

        private void shelf_trackBar2_Scroll(object sender, EventArgs e)
        {

            lblShelf.Text = $"Shelf: {tbShelf.Value}";
        }

        private void wall_trackBar1_Scroll(object sender, EventArgs e)
        {
            lblWall.Text = $"Wall: {tbWall.Value}";
        }

        private void pageRtb_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
