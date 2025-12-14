using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;

namespace LibraryOfBabel
{
    public partial class Form1 : Form
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz ,."; // remove uppercase letters
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
            string hex = "0";
            rtbHex.Text = hex;
            GoToPage(hex, 1, 1, 1, 1);
            LoadInjectedPages();

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
        string MakeKey(string hex, int wall, int shelf, int volume, int page)
        {
            return $"{hex}|{wall}|{shelf}|{volume}|{page}";
        }
        private Dictionary<string, string> injectedPages =
            new Dictionary<string, string>();

        private readonly string InjectedPagesPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "injected_pages.json");

        void LoadInjectedPages()
        {
            if (File.Exists(InjectedPagesPath))
            {
                injectedPages = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    File.ReadAllText(InjectedPagesPath)
                ) ?? new Dictionary<string, string>();
            }
        }
        void SaveInjectedPages()
        {
            File.WriteAllText(
                InjectedPagesPath,
                JsonConvert.SerializeObject(injectedPages, Newtonsoft.Json.Formatting.Indented)
            );
        }

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
        private string lastInjectedPageText = null;
        private (string hex, int wall, int shelf, int volume, int page)? lastPhraseLocation = null;


        // Update button1_Click
        private void button1_Click(object sender, EventArgs e)
        {
            string phrase = txtSearch.Text.Trim().ToLower(); // convert to lowercase
            if (phrase.Length == 0)
            {
                MessageBox.Show("Enter a phrase to search.");
                return;
            }

            //lastSearchedPhrase = phrase; // store lowercase phrase

            var loc = LocatePhrase(phrase);

            string pageText = GeneratePageWithPhrase(
                loc.hex, loc.wall, loc.shelf, loc.volume, loc.page,
                phrase, loc.insertIndex
            );

            string key = MakeKey(loc.hex, loc.wall, loc.shelf, loc.volume, loc.page);

            // STORE IT
            injectedPages[key] = pageText;
            SaveInjectedPages();

            // DISPLAY
            rtbOutput.Text = pageText;



            rtbOutput.Text =
                $"Your phrase exists at:\nHex: {loc.hex}\nWall: {loc.wall}\nShelf: {loc.shelf}\nVolume: {loc.volume}\nPage: {loc.page}\n\n=== PAGE TEXT ===\n{pageText}";

            // Highlight phrase
            int index = rtbOutput.Text.IndexOf(phrase, StringComparison.Ordinal);
            if (index >= 0)
            {
                rtbOutput.Select(index, phrase.Length);
                rtbOutput.SelectionColor = System.Drawing.Color.Red;
                rtbOutput.SelectionBackColor = System.Drawing.Color.Transparent;
                rtbOutput.Select(0, 0);
            }

            // Update UI
            lblVolume.Text = "Volume: " + loc.volume;
            tbVolume.Value = loc.volume;
            lblShelf.Text = "Shelf: " + loc.shelf;
            tbShelf.Value = loc.shelf;
            lblWall.Text = "Wall: " + loc.wall;
            tbWall.Value = loc.wall;
            pageRtb.Text = loc.page.ToString();
            rtbHex.Text = loc.hex;

            //lastPhraseLocation = loc; // store location
        }

        private (string hex, int wall, int shelf, int volume, int page, int insertIndex) LocatePhrase(string phrase)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(phrase));

                string hex = BitConverter.ToString(hash).Replace("-", "").Substring(0, 20).ToLower();

                int wall = (hash[20] % 4) + 1;
                int shelf = (hash[21] % 5) + 1;
                int volume = (hash[22] % 32) + 1;
                int page = (hash[23] % 410) + 1;
                int insertIndex = hash[24] % (PageLength - phrase.Length);

                return (hex, wall, shelf, volume, page, insertIndex);
            }
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

            // Generate the key for this location
            string key = MakeKey(hex, wall, shelf, volume, page);

            // Check if this page has an injected phrase
            if (injectedPages.ContainsKey(key))
            {
                string injectedText = injectedPages[key];
                rtbOutput.Text =
                    $"Location:\nHex: {hex}\nWall: {wall}\nShelf: {shelf}\nVolume: {volume}\nPage: {page}\n\n=== PAGE TEXT ===\n{injectedText}";

                // Store last injected page for reference/highlighting
                lastInjectedPageText = injectedText;
                lastPhraseLocation = (hex, wall, shelf, volume, page);

                // Highlight phrase if the search box contains text
                string phrase = txtSearch.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(phrase))
                {
                    int index = rtbOutput.Text.IndexOf(phrase, StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        rtbOutput.Select(index, phrase.Length);
                        rtbOutput.SelectionColor = System.Drawing.Color.Red;
                        rtbOutput.SelectionBackColor = System.Drawing.Color.Transparent;
                        rtbOutput.Select(0, 0);
                    }
                }
                return;
            }

            // If no injected phrase exists, just display normal page
            lastInjectedPageText = null;
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

        private void randButton2_Click(object sender, EventArgs e)
        {
            var loc = RandomLocation();
            GoToPage(loc.hex, loc.wall, loc.shelf, loc.volume, loc.page);

            // Update the UI controls
            rtbHex.Text = loc.hex;
            tbWall.Value = loc.wall;
            tbShelf.Value = loc.shelf;
            tbVolume.Value = loc.volume;
            pageRtb.Text = loc.page.ToString();
            lblWall.Text = $"Wall: {loc.wall}";
            lblShelf.Text = $"Shelf: {loc.shelf}";
            lblVolume.Text = $"Volume: {loc.volume}";
        }

    }
}
