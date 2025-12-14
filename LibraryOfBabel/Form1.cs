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
            string hex = "0000000000000000000000000000000000000000000000000000000000000000";
            rtbHex.Text = hex;
            GoToPage(hex, 1, 1, 1, 1);
            LoadInjectedPages();

        }
        string MakeHexWithPhraseAndLocation(
    string hex, int wall, int shelf, int volume, int page, int insertIndex, string phrase)
        {
            string encodedPhrase = string.IsNullOrEmpty(phrase)
                ? ""
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(phrase));

            // Fixed-width hex: wall(2) + shelf(2) + volume(2) + page(3) + insertIndex(4)
            string locationHex = $"{wall:X2}{shelf:X2}{volume:X2}{page:X3}{insertIndex:X4}";

            return hex.Substring(0, 64) + locationHex + encodedPhrase;
        }

        (string hex, int wall, int shelf, int volume, int page, int insertIndex, string phrase)
        ParseHexWithPhraseAndLocation(string hexWithPhrase)
        {
            // Ensure the string is at least 64 chars for the SHA256 part
            if (hexWithPhrase.Length < 64)
                throw new ArgumentException("Hex string too short.");

            string hex = hexWithPhrase.Substring(0, 64);

            // Default values if location info is missing
            int wall = 1, shelf = 1, volume = 1, page = 1, insertIndex = 0;
            string phrase = "";

            if (hexWithPhrase.Length >= 77) // full location info is present
            {
                wall = Convert.ToInt32(hexWithPhrase.Substring(64, 2), 16);
                shelf = Convert.ToInt32(hexWithPhrase.Substring(66, 2), 16);
                volume = Convert.ToInt32(hexWithPhrase.Substring(68, 2), 16);
                page = Convert.ToInt32(hexWithPhrase.Substring(70, 3), 16);
                insertIndex = Convert.ToInt32(hexWithPhrase.Substring(73, 4), 16);

                if (hexWithPhrase.Length > 77) // phrase is present
                {
                    string encodedPhrase = hexWithPhrase.Substring(77);
                    try { phrase = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPhrase)); }
                    catch { phrase = ""; }
                }
            }

            return (hex, wall, shelf, volume, page, insertIndex, phrase);
        }

        private void GoToPage(string hexWithPhrase, int currentWall, int currentShelf, int currentVolume, int currentPage)
        {
            var (hex, savedWall, savedShelf, savedVolume, savedPage, insertIndex, phrase) =
                ParseHexWithPhraseAndLocation(hexWithPhrase);

            string pageText;

            // Only insert phrase if the current location matches the saved location
            if (!string.IsNullOrEmpty(phrase) &&
                currentWall == savedWall && currentShelf == savedShelf &&
                currentVolume == savedVolume && currentPage == savedPage)
            {
                pageText = GeneratePageWithPhrase(hex, currentWall, currentShelf, currentVolume, currentPage, phrase, insertIndex);
            }
            else
            {
                pageText = GeneratePage(hex, currentWall, currentShelf, currentVolume, currentPage);
            }

            rtbOutput.Text =
                $"Location:\nHex: {hex}\nWall: {currentWall}\nShelf: {currentShelf}\nVolume: {currentVolume}\nPage: {currentPage}\n\n=== PAGE TEXT ===\n{pageText}";

            // Highlight phrase only if inserted
            if (!string.IsNullOrEmpty(phrase) &&
                currentWall == savedWall && currentShelf == savedShelf &&
                currentVolume == savedVolume && currentPage == savedPage)
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

            // Update UI controls
            rtbHex.Text = hexWithPhrase;
            tbWall.Value = currentWall;
            tbShelf.Value = currentShelf;
            tbVolume.Value = currentVolume;
            pageRtb.Text = currentPage.ToString();
            lblWall.Text = $"Wall: {currentWall}";
            lblShelf.Text = $"Shelf: {currentShelf}";
            lblVolume.Text = $"Volume: {currentVolume}";
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
        // Encode a phrase into the hex string
        // Encode a phrase into the hex string
        string MakeHexWithPhrase(string hex, string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return hex;

            // Base64 encode the phrase so it can safely be appended to the hex
            string encodedPhrase = Convert.ToBase64String(Encoding.UTF8.GetBytes(phrase));
            return hex.Substring(0, 64) + encodedPhrase;
        }

        // Decode the hex string back into original hex and phrase
        (string hex, string phrase) ParseHexWithPhrase(string hexWithPhrase)
        {
            string hex = hexWithPhrase.Substring(0, 64);
            string phrase = "";

            if (hexWithPhrase.Length > 64)
            {
                string encodedPhrase = hexWithPhrase.Substring(64);
                try
                {
                    phrase = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPhrase));
                }
                catch
                {
                    // Invalid base64 → ignore
                    phrase = "";
                }
            }

            return (hex, phrase);
        }

        private (string hex, int wall, int shelf, int volume, int page) RandomLocation()
        {
            Random r = new Random();

            int hexLength = 64; // full SHA-256 length in hex
            string hex = "";
            for (int i = 0; i < hexLength; i++)
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

                string hex = BitConverter.ToString(hash).Replace("-", "").ToLower();

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
            string phrase = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(phrase))
            {
                MessageBox.Show("Enter a phrase to search.");
                return;
            }

            // Get the deterministic location and hex for this phrase
            var loc = LocatePhrase(phrase);

            // Build a hex string that contains the phrase and location info
            string hexWithPhrase = MakeHexWithPhraseAndLocation(
                loc.hex, loc.wall, loc.shelf, loc.volume, loc.page, loc.insertIndex, phrase
            );

            // Update the hex textbox so the UI reflects the new hex
            rtbHex.Text = hexWithPhrase;

            // Go to the page deterministically
            GoToPage(hexWithPhrase, loc.wall, loc.shelf, loc.volume, loc.page);
        }




        private (string hex, int wall, int shelf, int volume, int page, int insertIndex) LocatePhrase(string phrase)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(phrase));

                string hex = BitConverter.ToString(hash).Replace("-", "").ToLower();

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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text.Length > 3200)
            {
                txtSearch.Text = txtSearch.Text.Substring(0, 3200);
                txtSearch.SelectionStart = txtSearch.Text.Length; // keep cursor at end
                MessageBox.Show("Phrase cannot exceed 3200 characters.");
            }
        }
    }
}
