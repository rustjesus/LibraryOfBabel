using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace LibraryOfBabel
{
    public partial class Form1 : Form
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz ,.";
        private const int PageLength = 3200;

        public Form1()
        {
            InitializeComponent();
            rtbOutput.ReadOnly = true;
            rtbOutput.Cursor = Cursors.Default;
            rtbOutput.ShortcutsEnabled = false;
            rtbOutput.TabStop = false;
            rtbOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;


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

                int insertIndex = hash[24] % (PageLength - phrase.Length);

                return (hex, wall, shelf, volume, page, insertIndex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string phrase = txtSearch.Text.Trim().ToLower();

            if (phrase.Length == 0)
            {
                MessageBox.Show("Enter a phrase to search.");
                return;
            }

            // Deterministic location lookup
            var loc = LocationFromPhrase(phrase);

            // Generate the page guaranteed to contain the phrase
            string pageText = GeneratePageWithPhrase(
                loc.hex, loc.wall, loc.shelf, loc.volume, loc.page,
                phrase, loc.insertIndex);
            // Display results
            rtbOutput.Text =
                $"Your phrase exists at:\n" +
                $"Hex: {loc.hex}\n" +
                $"Wall: {loc.wall}\n" +
                $"Shelf: {loc.shelf}\n" +
                $"Volume: {loc.volume}\n" +
                $"Page: {loc.page}\n" +
                $"Index: {loc.insertIndex}\n\n" +
                $"=== PAGE TEXT ===\n" +
                $"{pageText}";

            // Highlight phrase in red
            int index = rtbOutput.Text.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                rtbOutput.Select(index, phrase.Length);
                rtbOutput.SelectionColor = System.Drawing.Color.Red;
                rtbOutput.SelectionBackColor = System.Drawing.Color.Transparent;
                rtbOutput.Select(0, 0);
            }

        }

    }
}
