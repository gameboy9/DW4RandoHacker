using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace DW4RandoHacker
{
    public partial class Form1 : Form
    {
        bool loading = true;
        byte[] romData;
        byte[] romData2;

        int[] monsterRank = // after 0x55, 0x??????, - bisonhawk unknown
            {
                    0x5c, 0x01, 0x00, 0x03, 0x02, 0x05, 0x08, 0x07, 0x09, 0x06, 0x0b, 0x0e, 0x0a, 0x11, 0x0d, 0x0f, // 6
                    0x14, 0x0c, 0x1c, 0x1a, 0x18, 0x13, 0x10, 0x1f, 0x26, 0x16, 0x1e, 0x19, 0x17, 0x24, 0x1b, 0x22, // 15
                    0x23, 0x15, 0x1d, 0x2a, 0x20, 0x27, 0x25, 0x21, 0x43, 0x28, 0x2f, 0x04, 0x31, 0x2c, 0x3c, 0x29, // 27
                    0x3d, 0x2d, 0x36, 0x45, 0x2e, 0x38, 0x39, 0x33, 0x42, 0x3e, 0x58, 0x4d, 0x40, 0x4a, 0x32, 0x47, // 45
                    0x2b, 0x35, 0x52, 0x48, 0x46, 0x37, 0x4c, 0xaf, 0x34, 0x5e, 0x3a, 0x4f, 0x66, 0xb3, 0x3b, 0x49, // 77
                    0xb0, 0xb1, 0x56, 0x41, 0x51, 0x50, 0x55, 0x57, 0x44, 0x5a, 0x3f, 0xb2, 0xba, 0x30, 0x53, 0x60, // 104
                    0x5b, 0x75, 0x5f, 0x4b, 0x68, 0x63, 0x5d, 0x54, 0x6b, 0x61, 0x67, 0x6e, 0x6a, 0x76, 0x12, 0x69,
                    0x70, 0x59, 0x78, 0x72, 0xab, 0x7c, 0x7d, 0x73, 0x71, 0x6c, 0x6f, 0x7f, 0x77, 0x74, 0x64, 0x86,
                    0x7a, 0x6d, 0x79, 0x7b, 0x80, 0x81, 0x7e, 0x87, 0x83, 0x88, 0x85, 0x82, 0x84, 0x62, 0x8c, 0x97,
                    0x89, 0x8a, 0x8d, 0x8b, 0x93, 0x90, 0xc0, 0x9c, 0x8e, 0x98, 0x95, 0xb4, 0x96, 0x9f, 0x9a, 0x99,
                    0x92, 0x9d, 0xa2, 0x94, 0x9e, 0x91, 0x8f, 0xa1, 0xa9, 0x9b, 0xa0, 0xa6, 0xaa, 0xac, 0x65, 0xa8,
                    0xb8, 0xa3, 0xa4, 0xa5, 0xa7, 0xbf, 0xb9, 0xbe, 0xb7, 0xb6, 0xb5, 0xc1, 0xc2, 0xbc // 15000 - bosses start at 0xb5
            };


        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog1.FileName;
                runChecksum();
            }
        }

        private void runChecksum()
        {
            try
            {
                using (var md5 = SHA1.Create())
                {
                    using (var stream = File.OpenRead(txtFileName.Text))
                    {
                        lblSHAChecksum.Text = BitConverter.ToString(md5.ComputeHash(stream)).ToLower().Replace("-", "");
                    }
                }
            }
            catch
            {
                lblSHAChecksum.Text = "????????????????????????????????????????";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboXPAdjustment.SelectedIndex = 1;
            cboGoldAdjustment.SelectedIndex = 1;

            txtSeed.Text = (DateTime.Now.Ticks % 2147483647).ToString();
            bool namesRead = false;

            try
            {
                using (TextReader reader = File.OpenText("lastFile4.txt"))
                {
                    txtFileName.Text = reader.ReadLine();
                    txtCompare.Text = reader.ReadLine();
                    txtC1Name1.Text = reader.ReadLine();
                    txtC1Name2.Text = reader.ReadLine();
                    txtC2Name1.Text = reader.ReadLine();
                    txtC2Name2.Text = reader.ReadLine();
                    txtC2Name3.Text = reader.ReadLine();
                    txtC3Name1.Text = reader.ReadLine();
                    txtC3Name2.Text = reader.ReadLine();
                    txtC3Name3.Text = reader.ReadLine();
                    txtC4Name1.Text = reader.ReadLine();
                    txtC4Name2.Text = reader.ReadLine();
                    txtC4Name3.Text = reader.ReadLine();
                    txtC5Name1.Text = reader.ReadLine();
                    txtC5Name2.Text = reader.ReadLine();
                    txtC5Name3.Text = reader.ReadLine();
                    txtC5Name4.Text = reader.ReadLine();
                    namesRead = true;

                    txtSeed.Text = reader.ReadLine();
                    txtFlags.Text = reader.ReadLine();
                    determineChecks(null, null);

                    runChecksum();
                    loading = false;
                }
            }
            catch
            {
                // ignore error
                if (!namesRead)
                {
                    txtC1Name1.Text = "Ragnar";
                    txtC1Name2.Text = "Healie";
                    txtC2Name1.Text = "Alena";
                    txtC2Name2.Text = "Cristo";
                    txtC2Name3.Text = "Brey";
                    txtC3Name1.Text = "Taloon";
                    txtC3Name2.Text = "Laurent";
                    txtC3Name3.Text = "Strom";
                    txtC4Name1.Text = "Mara";
                    txtC4Name2.Text = "Nara";
                    txtC4Name3.Text = "Orin";
                    txtC5Name1.Text = "Hector";
                    txtC5Name2.Text = "Panon";
                    txtC5Name3.Text = "Lucia";
                    txtC5Name4.Text = "Doran";
                }
            }
        }

        private void btnNewSeed_Click(object sender, EventArgs e)
        {
            txtSeed.Text = (DateTime.Now.Ticks % 2147483647).ToString();
        }

        private void btnRandomize_Click(object sender, EventArgs e)
        {
            if (lblSHAChecksum.Text != lblReqChecksum.Text)
            {
                if (MessageBox.Show("The checksum of the ROM does not match the required checksum.  Patch anyway?", "Checksum mismatch", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            if (!loadRom())
                return;

            hackRom();

            //// All ROM hacks will revive ALL characters on a ColdAsACod.
            //// There will be a temporary graphical error if you use less than four characters, but I'm going to leave it be.
            //byte[] codData1 = { 0xa0, 0x00, // Make sure Y is 0 first.
            //    0xb9, 0x3c, 0x07,
            //    0xc9, 0x80,
            //    0x90, 0x03, // If less than 0x80, skip.
            //    0x20, 0xb2, 0xbf, // JSR to a bunch of unused code, which will have the "revive one character code" that I'm replacing.
            //    0xc8, 0xc8, // Increment Y twice (Y is used to revive the characters)
            //    0xc0, 0x08, // Compare Y with 08
            //    0xd0, 0xf0, // If not equal, go back to the JSR mentioned above
            //    0xa0, 0x00, // Set Y back to 0 to make sure the game doesn't think something is up
            //    0xea, 0xea, 0xea, 0xea, 0xea,
            //    0xea, 0xea, 0xea, 0xea, 0xea,
            //    0xea, 0xea }; // 12 NOPs, since I have nothing else to do.
            //byte[] codData2 = { 0xa9, 0x80, // Load 80, the status for alive
            //    0x99, 0x3c, 0x07, // store to two status bytes
            //    0x99, 0x3d, 0x07,
            //    0xb9, 0x24, 0x07, // Load max HP
            //    0x99, 0x1c, 0x07, // save max HP
            //    0xb9, 0x25, 0x07, // second byte
            //    0x99, 0x1d, 0x07,
            //    0xb9, 0x34, 0x07, // Load max MP
            //    0x99, 0x2c, 0x07, // save max MP
            //    0xb9, 0x35, 0x07, // second byte
            //    0x99, 0x2d, 0x07,
            //    0x60 }; // end JSR

            //for (int lnI = 0; lnI < codData1.Length; lnI++)
            //    romData[0x22b3 + lnI] = codData1[lnI];
            //for (int lnI = 0; lnI < codData2.Length; lnI++)
            //    romData[0x3fc2 + lnI] = codData2[lnI];

            saveRom();
        }

        private bool hackRom()
        {
            if (chkRandomTreasures.Checked || chkRandomMonsterStats.Checked)
                if (MessageBox.Show(this, "WARNING:  Random monster stats and attacks and especially random treasures is still experimental and may cause crashes where the game cannot be winnable.  Continue?", "DW4 RandoHacker", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;

            Random r1;
            try
            {
                if (txtSeed.Text == "whoa") r1 = new Random((int)DateTime.Now.Ticks % 2147483647);
                else r1 = new Random(int.Parse(txtSeed.Text));
            }
            catch
            {
                MessageBox.Show("Invalid seed.  It must be a number from 0 to 2147483648.");
                return false;
            }

            for (int lnI = 0; lnI < 8; lnI++)
                romData[0x49145 + lnI] = 0; // Make sure all characters are loaded right away; otherwise, Chapter 1 will most likely start with a ghost.

            if (chkSoloHero.Checked)
            {
                byte power = 0;
                if ((string)cboSoloHero.SelectedItem == "Hero") power = 0;
                if ((string)cboSoloHero.SelectedItem == "Cristo") power = 1;
                if ((string)cboSoloHero.SelectedItem == "Nara") power = 2;
                if ((string)cboSoloHero.SelectedItem == "Mara") power = 3;
                if ((string)cboSoloHero.SelectedItem == "Brey") power = 4;
                if ((string)cboSoloHero.SelectedItem == "Taloon") power = 5;
                if ((string)cboSoloHero.SelectedItem == "Ragnar") power = 6;
                if ((string)cboSoloHero.SelectedItem == "Alena") power = 7;
                if (chkSoloCanEquipAll.Checked)
                    for (int lnI = 0; lnI < 80; lnI++)
                        romData[0x40c75 + lnI] = (byte)Math.Pow(2, power); // Going to make sure the character gets to equip everything!
                else
                { // still have to allow equipping of the Zenethian equipment so you can get through the tower and castle...
                    romData[0x40c75 + 0x14] = (byte)Math.Pow(2, power);
                    romData[0x40c75 + 0x21] = (byte)Math.Pow(2, power);
                    romData[0x40c75 + 0x37] = (byte)Math.Pow(2, power);
                    romData[0x40c75 + 0x44] = (byte)Math.Pow(2, power);
                    romData[0x40c75 + 0x4b] = (byte)Math.Pow(2, power);
                }
                for (int lnI = 0; lnI < 5; lnI++)
                    romData[0x4914d + lnI] = (byte)(128 + power); // This will ensure the same character starts each chapter.

                // This makes sure all item choices point to the right person's inventory. (you get tripped up in that in Chapter 3's Armor charity in Bonmalmo at a minimum)
                romData[0x41f93] = (byte)(1 + (30 * power));
                romData[0x41f95] = (byte)(1 + (30 * power));
                romData[0x41f97] = (byte)(1 + (30 * power));
                romData[0x41f99] = (byte)(1 + (30 * power));
                romData[0x41f9b] = (byte)(1 + (30 * power));
                romData[0x41f9d] = (byte)(1 + (30 * power));
                romData[0x41f9f] = (byte)(1 + (30 * power));
                romData[0x41fa1] = (byte)(1 + (30 * power));

                // Double the HP gain
                romData[0x49e22] = 0x20; // JSR to an unused portion of the rom
                romData[0x49e23] = 0x68;
                romData[0x49e24] = 0xbf;
                romData[0x4bf78] = 0x0a; // Arithmetic shift left... multipling the vitality gain by 4 instead of 2
                romData[0x4bf79] = 0x8d; // Store accumulator absolute -> 6e09
                romData[0x4bf7a] = 0x09;
                romData[0x4bf7b] = 0x6e;
                romData[0x4bf7c] = 0x60; // end subroutine

                // Remove baseline - otherwise, you can't double HP(until I figure out how to double the HP baseline...)
                romData[0x49df6] = 0xb0;
                romData[0x49df7] = 0x06;

                // Need to dodge a check to make sure you can have multiple of the same person in the party.
                romData[0x41338] = 0xea;
                romData[0x41339] = 0xea;

                romData[0x4af70] = 0xbf; // Make the medical herb super powerful out of battle...
                romData[0x4f7f9] = 0xff; // ... and even more powerful in battle! (next 2 lines)
                romData[0x4f7fa] = 0xff;

                // Ensure that ? allies become the solo hero instead.
                for (int lnI = 0; lnI < 6; lnI++)
                    romData[0x413ea + lnI] = 0xea;
                romData[0x413f1] = (byte)(128 + power);
                romData[0x413fc] = 0x4c;
                romData[0x413fd] = 0x9c;
                romData[0x413fe] = 0x93;

                // Make the Cristo and Brey join change to the solo hero joining twice instead in Chapter 2.
                romData[0x79d44] = power;
                romData[0x79d49] = power;

                // Force the solo hero to fight in Chapter 2's tournament.  This isn't completely neccessary, but it will smooth out processing.
                romData[0x79074] = power;

                // Make the Bonmalmo armor shop and the Endor owned shop point to the Chapter 3 heroes inventory.  This isn't neccessary, but it will smooth out processing.
                romData[0x55ee9] = romData[0x56562] = romData[0x565d5] = power;
                romData[0x5654e] = power;

                // Turn Nara into the solo hero in Chapter 4!
                romData[0x76b3c] = power;

                // You have to gain the magic key because having Orin in the party destroys the solo hero concept.  
                // We'll replace the first treasure chest in the Aktemto Mine with the Magic Key.
                //romData[0x7bef1] = 0x72;
                romData[0x7b399] = power;

                // Force Nara to solo hero in Chapter 5
                romData[0x77903] = power;
                // Force Mara to solo hero in Chapter 5
                romData[0x54ad7] = power; // Otherwise, Mara will think that Nara isn't a part of the party...
                romData[0x77909] = power;

                // Dodge an issue with Chapter 5's Cave Of Betrayal
                romData[0x41423] = 0xa0;
                romData[0x41424] = 0x00; // LDY $#00 - This will force whoever is in the lead to survive "the drop"
                romData[0x41425] = 0xea;

                // Force Taloon to solo hero in Chapter 5
                romData[0x732ad] = power;

                // Force solo Brey to solo hero in Chapter 5
                romData[0x7362a] = power;

                // Force Alena, Brey, and Cristo to solo hero in Chapter 5
                romData[0x739d9] = power;
                romData[0x739de] = power;
                romData[0x739e9] = power;

                romData[0x41ad4] = (byte)(128 + power); // better yet, just fill the wagon full of solo heroes... you only need four... but sometimes it winds up less than four, and we don't want that.
                romData[0x41ad6] = 0x02; // LDY #$02 - it was #$09 - that causes a graphics crash with the line above.
                romData[0x41aa9] = 0x02; // Required to assure a wagon full of solo heroes.

                // A bunch of weird stuff preventing the Keeleon battle from occuring... (Ragnar demanding "the hero"...)
                romData[0x54e08] = power;
                romData[0x7724e] = power;
                // ... and Ragnar joining...
                romData[0x73634] = power;

                // This gets us past the Zenethian Helm block...
                romData[0x56c1d] = power; // Panon -> solo hero

                // This passes the Zenithian checks...
                romData[0x774cb] = power; // To enter the tower...
                romData[0x2377a] = power; // And the castle...

                // Only one unit of experience points - otherwise Taloon could get 10 times the experience points earned.
                //romData[0x41ff1] = 0x00;
            }

            int finalHero = 0;
            if (chkC14Random.Checked && !chkSoloHero.Checked)
            {
                // Randomize the starting character for each chapter...
                // Come up with eight distinct numbers...
                int[] heroes = { 0, 1, 2, 3, 4, 5, 6, 7 };
                for (int lnI = 0; lnI < 100; lnI++)
                {
                    int numberToSwap1 = (r1.Next() % 8);
                    int numberToSwap2 = (r1.Next() % 8);
                    int swappy = heroes[numberToSwap1];
                    heroes[numberToSwap1] = heroes[numberToSwap2];
                    heroes[numberToSwap2] = swappy;
                }

                for (int lnI = 0; lnI < 5; lnI++)
                    romData[0x4914d + lnI] = (byte)(128 + heroes[lnI]); // This will ensure the same character starts each chapter.

                // Make Taloon sane in the first 4 chapters, not just in Chapter 3.
                if (heroes[0] == 5) romData[0x47309] = 0;
                else if (heroes[1] == 5 || heroes[5] == 5 || heroes[6] == 5) romData[0x47309] = 1;
                else if (heroes[2] == 5) romData[0x47309] = 2;
                else romData[0x47309] = 3;

                // Make the Cristo and Brey join change to the solo hero joining twice instead in Chapter 2.
                romData[0x79d44] = (byte)heroes[5];
                romData[0x79d49] = (byte)heroes[6];

                // Force the hero acting as the chapter 2 "princess" to fight in Chapter 2's tournament.  
                // This IS neccessary with random heroes... especially if Alena is acting as Cristo or Brey!
                romData[0x79074] = (byte)heroes[1];

                // Make the Bonmalmo armor shop and the Endor owned shop point to the Chapter 3 heroes inventory.
                romData[0x55ee9] = romData[0x56562] = romData[0x565d5] = (byte)heroes[2];
                romData[0x5654e] = (byte)heroes[2];

                // Force the heroes acting as the chapter 4 "Nara and Mara" get out of Chapter 4 successfully.
                romData[0x7907a] = (byte)heroes[3];
                romData[0x7907b] = (byte)heroes[7];

                // Turn Nara into the solo hero in Chapter 4!
                romData[0x76b3c] = (byte)heroes[7];

                finalHero = heroes[4];

                // Give the Thief's Key to the Chapter 5 hero to prevent a potential unwinnable condition.
                // romData[0x4911e] = (byte)heroes[7]; <------------ ... maybe...

                // Randomize the starting character for each chapter...
                // Come up with eight distinct numbers...
                //int[] heroes = { 0, 1, 2, 3, 4, 5, 6, 7 };

                for (int lnI = 0; lnI < 100; lnI++)
                {
                    int numberToSwap1 = (r1.Next() % 8);
                    int numberToSwap2 = (r1.Next() % 8);
                    int swappy = heroes[numberToSwap1];
                    heroes[numberToSwap1] = heroes[numberToSwap2];
                    heroes[numberToSwap2] = swappy;
                }

                //if (chkC14Random.Checked)
                //{
                for (int lnI = 0; lnI < 8; lnI++)
                {
                    if (heroes[lnI] == finalHero)
                    {
                        int swappy = heroes[lnI];
                        heroes[lnI] = heroes[0];
                        heroes[0] = swappy;
                    }
                }
                //}

                // Force Nara to solo hero in Chapter 5
                romData[0x77903] = (byte)heroes[1];
                // Force Mara to solo hero in Chapter 5
                romData[0x54ad7] = (byte)heroes[0]; // Otherwise, Mara will think that Nara isn't a part of the party...
                romData[0x77909] = (byte)heroes[2];

                // Dodge an issue with Chapter 5's Cave Of Betrayal
                romData[0x41423] = 0xa0;
                romData[0x41424] = 0x00; // LDY $#00 - This will force whoever is in the lead to survive "the drop"
                romData[0x41425] = 0xea;

                // Force Taloon to solo hero in Chapter 5
                romData[0x732ad] = (byte)heroes[3];

                // Force Alena, Brey, and Cristo to solo hero in Chapter 5
                romData[0x739d9] = (byte)heroes[4];
                romData[0x739de] = (byte)heroes[5]; // <----- Give thief's key to this person...
                romData[0x739e9] = (byte)heroes[6];

                // A bunch of weird stuff preventing the Keeleon battle from occuring... (Ragnar demanding "the hero"...)
                romData[0x54e08] = (byte)heroes[0];
                romData[0x7724e] = (byte)heroes[0];
                // ... and Ragnar joining...
                romData[0x73634] = (byte)heroes[7];

                // Give the Thief's Key to the Chapter 5 hero to prevent a potential unwinnable condition.
                romData[0x4911e] = (byte)heroes[0];

                // Randomize NPCs
                for (int lnI = 0; lnI < 100; lnI++)
                {
                    int numberToSwap1 = (r1.Next() % 8);
                    int numberToSwap2 = (r1.Next() % 8);
                    int swappy = heroes[numberToSwap1];
                    heroes[numberToSwap1] = heroes[numberToSwap2];
                    heroes[numberToSwap2] = swappy;
                }

                // 0x778df (8), 0x778f1 (9), 0x778f7 (a), 0x778fd (b), 0x73678 (c), 0x7364d (d), 0x735de (e), 0x73325 (f)
                // Just do the first four for now.  The last four is VERY complex... 
                // (acquisition is fine, but Hector leaving, Panon leaving, Panon joking, Lucia leaving, and the Doran monster detection are the tough parts)
                int[] npcs = { 0x778df, 0x778f1, 0x778f7, 0x778fd };
                int npcMark = 0;
                for (int lnI = 0; lnI < heroes.Length; lnI++)
                {
                    if (heroes[lnI] <= 3)
                    {
                        romData[npcs[npcMark]] = (byte)(heroes[lnI] + 8);
                        if (npcs[npcMark] == 0x778f1)
                            romData[0x7b399] = (byte)(heroes[lnI] + 8);
                        npcMark++;
                    }
                }
            }

            //if (chkC5Random.Checked && !chkSoloHero.Checked)
            //{
            //}

            // Make Chapter 2 adjustments if requested.
            if (chkCh2AwardXPTournament.Checked)
            {
                romData[0x60054 + (0xaf * 22) + 2] = 60;
                romData[0x60054 + (0xb0 * 22) + 2] = 80;
                romData[0x60054 + (0xb1 * 22) + 2] = 80;
                romData[0x60054 + (0xb2 * 22) + 2] = 100;
                romData[0x60054 + (0xba * 22) + 2] = 100;
            }

            // Make Chapter 3 adjustments if requested.
            // Make the shop one piece of gold... I still think that Chapter 3 sucks.  :)
            if (chkShop1.Checked)
            {
                romData[0x5603c] = 0x00;
                romData[0x56044] = 0x01;
            }
            if (chkShop25K.Checked)
            {
                romData[0x5603c] = 0x61;
                romData[0x56044] = 0xa8;
            }
            // This can make the tunnel one piece of gold...
            if (chkTunnel1.Checked)
            {
                romData[0x56641] = 0x00;
                romData[0x56645] = 0x01;
            }

            // Now adjust XP for all monsters...
            for (int lnI = 0; lnI <= 0xc2; lnI++)
            {
                double xp = (romData[0x60056 + (lnI * 22) + 1] * 256) + romData[0x60056 + (lnI * 22) + 0];
                double origXP = xp;
                if ((string)cboXPAdjustment.SelectedItem == "50%") xp = xp / 2;
                if ((string)cboXPAdjustment.SelectedItem == "150%") xp = xp * 3 / 2;
                if ((string)cboXPAdjustment.SelectedItem == "200%") xp = xp * 2;
                if ((string)cboXPAdjustment.SelectedItem == "250%") xp = xp * 5 / 2;
                if ((string)cboXPAdjustment.SelectedItem == "300%") xp = xp * 3;
                if ((string)cboXPAdjustment.SelectedItem == "400%") xp = xp * 4;
                if ((string)cboXPAdjustment.SelectedItem == "500%") xp = xp * 5;
                if (txtSeed.Text == "whoa") xp = 65000;

                if (chkXPRandom.Checked && txtSeed.Text != "whoa")
                    xp = (r1.Next() % (xp * 2));

                int xpTrue = (int)Math.Round(xp);
                if (xpTrue < 1 && origXP >= 1) xpTrue = 1;
                if (xpTrue > 65000) xpTrue = 65000;
                romData[0x60056 + (lnI * 22) + 1] = (byte)(xpTrue / 256);
                romData[0x60056 + (lnI * 22) + 0] = (byte)(xpTrue % 256);
            }

            // Then the gold for all monsters...
            for (int lnI = 0; lnI <= 0xc2; lnI++)
            {
                double xp = ((romData[0x60056 + (lnI * 22) + 18] % 4) * 256) + (romData[0x60056 + (lnI * 22) + 7]);
                double origXP = xp;
                if ((string)cboGoldAdjustment.SelectedItem == "50%") xp = xp / 2;
                if ((string)cboGoldAdjustment.SelectedItem == "150%") xp = xp * 3 / 2;
                if ((string)cboGoldAdjustment.SelectedItem == "200%") xp = xp * 2;
                if (txtSeed.Text == "whoa") xp = 1000;

                if (chkGoldRandom.Checked && txtSeed.Text != "whoa")
                    xp = (r1.Next() % (xp * 2));

                int xpTrue = (int)Math.Round(xp);
                if (xpTrue < 1 && origXP >= 1) xpTrue = 1;
                if (xpTrue > 1000) xpTrue = 1000;
                romData[0x60056 + (lnI * 22) + 18] -= (byte)(romData[0x60054 + (lnI * 22) + 20] % 4);
                romData[0x60056 + (lnI * 22) + 18] += (byte)(xpTrue / 256);
                romData[0x60056 + (lnI * 22) + 7] = (byte)(xpTrue % 256);
            }

            if (chkRandomMonsterStats.Checked)
                randomizeMonsterStats(r1);

            if (chkRandomMonsterAttacks.Checked)
                randomizeMonsterAttacks(r1);

            if (chkRandomMonsterResistances.Checked)
                randomizeMonsterResistances(r1);

            if (chkRandomTreasures.Checked)
                randomizeTreasures(r1);

            // Finally, the encounter rate.  I've noticed that the encounter rate by default is VARIABLE!
            // 25% of normal = Branca Castle, north to the Heroes' Hometown, the approach to Necrosaro itself, and the Gardenbur Cave.  (the later two is guaranteed 1/64)
            // Part of the Zenithian Tower, Necrosaro's Castle, and Santeem Castle is 1/32 guaranteed.  (50% of normal)
            // The other part of Zenithian Tower is 1/24 guaranteed.  (75% of normal)
            // Frenor, the zones just outside of Colossus (not Dire Castle), Loch Tower (Chapter 1), Konenber to the Great Lighthouse, and Stancia all have a +25% encounter rate.
            // This makes Loch Tower a guaranteed 1/12.8 encounter rate.
            // Santeem and Endor in Chapter 2, and Lakanaba and the approach to the Cave Of Betrayal in Chapter 5 all have a +50% encounter rate.
            // The area outside of Tempe in Chapter 2 has a +75% encounter rate.
            // Finally, the big desert you must go through after acquiring the wagon has a +100% encounter rate.  This also may be why Fairy Water doesn't work through there.
            // I believe that makes the encounter rate 1/6.4 through there!
            for (int lnI = 0; lnI < 16; lnI++)
            {
                double encounterRate = (romData[0x6228b + lnI]);
                if ((string)cboEncounterRate.SelectedItem == "1/4") encounterRate = Math.Round(encounterRate / 4);
                if ((string)cboEncounterRate.SelectedItem == "1/3") encounterRate = Math.Round(encounterRate / 3);
                if ((string)cboEncounterRate.SelectedItem == "1/2") encounterRate = Math.Round(encounterRate / 2);
                if ((string)cboEncounterRate.SelectedItem == "2/3") encounterRate = Math.Round(encounterRate * 2 / 3);
                if ((string)cboEncounterRate.SelectedItem == "x1.5") encounterRate = Math.Round(encounterRate * 3 / 2);
                if ((string)cboEncounterRate.SelectedItem == "x2") encounterRate = Math.Round(encounterRate * 2);
                if ((string)cboEncounterRate.SelectedItem == "x2.5") encounterRate = Math.Round(encounterRate * 5 / 2);
                if ((string)cboEncounterRate.SelectedItem == "x3") encounterRate = Math.Round(encounterRate * 3);
                if ((string)cboEncounterRate.SelectedItem == "x4") encounterRate = Math.Round(encounterRate * 4);
                romData[0x6228b + lnI] = (byte)encounterRate;
            }

            if (chkRandomHeroEquip.Checked)
                randomizeHeroEquipment(r1);

            if (chkRandomMonsterZones.Checked)
                randomizeMonsterZones(r1);

            if (chkRandomStores.Checked)
                randomizeStores(r1);

            if (chkRandomHeroStats.Checked)
                randomizeHeroStats(r1);

            if (chkSpeedUpBattles.Checked)
            {
                romData[0x502ff] = 1; // instead of 4.  6 frames saved each enemy hit when there is one monster left.
                romData[0x5033f] = 2; // instead of 8.  6 frames saved each enemy hit where there are two or more monsters left.
                romData[0x50420] = 2; // instead of 12.  10 frames saved each time YOU are hit.
                romData[0x63237] = 2; // instead of 12.  10 frames saved each time an encounter begins.  (flash)
                romData[0x62eb8] = 1; // instead of 29.  28 frames saved each time an encounter begins.  (spiral)
                // Speed up the message speed
                romData[0x48624] = 0x01;
                romData[0x48625] = 0x04;
                romData[0x48626] = 0x08;
                romData[0x48627] = 0x0f;
                romData[0x48628] = 0x1f;
                romData[0x48629] = 0x2f;
                romData[0x4862a] = 0x3f;
            }

            // Give full control over all players in Chapter 5.  You lose the wagon control though.  I would LOVE to figure out how to get both though!  Maybe some nops?
            if (chkC5ControlAllChars.Checked)
                romData[0x46e1e] = 0x7f; // You can make it any number higher than 04, chapter 5 I think... 

            // Rename all characters - this starts at 0x2fba0
            string[] twoCharStrings = { "er", "on", "an", "or", "ar", "le", "ro", "al", "re", "st", "in", "ba", "ra", "ma", // 20s, starting at 22
                " s", "he", "ea", "en", "th", "el", "n ", "te", "et", "e ", "mo", "of", "ng", "at", "de", "f ", // 30s
                "rd", "ta", "ag", "me", " o", "ir", "ha", "la", "ni", "ce", "hi", "ic", "ll", "li", "sa", "nt", // 40s
                "do", "ia", "no", "to", "ur", "es", "ou", "pe", "rm", "as", "il", "ri", " h", " m", "s ", "ab", // 50s
                "be", "ee", "em", "go", "it", "l ", "ve", "dr", "ie", "ne", "r ", "wo", "ad", "ch", "ed", "nd", // 60s
                "se", "sh", "tr", " a", "bl", "fe", "ld", "nc", "ol", "os", "rn", "si", "vi", " b", " d", "am", // 70s
                "ge", "ig", "mi", "ot", "ti", "us", " c", "g ", "is", "lo", "od", "sw", "za", "ze", " r", "ac", // 80s
                "cl", "co", "d ", "gh", "ho", "io", "ke", "oo", "op", "so", "un", "y ", "ai", "bi", "cr", "da", // 90s
                "id", "im", "om", "pi", "po", "af", "ck", "ff", "gi", "gu", "ht", "iv", "rr", "sp", "ss", "t ", // a0s
                "ab", "bo", "ec", "fu", "na", "sl", " p", " w", "|s", "ak", "di", "fi", "f ", "iz", "ki", "lu", // b0s
                "mp", "nf", "rc", "av", "bb", "ca", "ef", "eo", "fa", "fl", "ga", "gr", "ly", "mb", "nu", "og", // c0s
                "ow", "pa", "pl", "pp", "ry", "sk", "tt", "tu", "wi", " k", " n", " t", "ay", "az", "a ", "br", // d0s
                "ds", "fo", "kn", "k ", "lm", "ns", "oa", "ob", "oi", "ph", "ty", "ul", "um", "ut", "wa", "au", // e0s
                "dt", "c ", "d ", "eb", "ep", "ev", "ey", "e ", "ip", "ka", "ko", "nb", "pr", "rt", "sc", "ua"}; // f0s

            int stringMarker = 0;
            for (int lnI = 0; lnI < 15; lnI++)
            {
                string name = "";
                name = ((lnI == 0 ? txtC2Name2.Text : lnI == 1 ? txtC4Name2.Text :
                    lnI == 2 ? txtC4Name1.Text : lnI == 3 ? txtC2Name3.Text : lnI == 4 ? txtC3Name1.Text :
                    lnI == 5 ? txtC1Name1.Text : lnI == 6 ? txtC2Name1.Text : lnI == 7 ? txtC1Name2.Text :
                    lnI == 8 ? txtC4Name3.Text : lnI == 9 ? txtC3Name2.Text : lnI == 10 ? txtC3Name3.Text :
                    lnI == 11 ? txtC5Name1.Text : lnI == 12 ? txtC5Name2.Text : lnI == 13 ? txtC5Name3.Text : txtC5Name4.Text)).ToLower();

                List<int> byteArray = new List<int>();
                for (int lnJ = 0; lnJ < name.Length; lnJ++)
                {
                    if (lnJ != name.Length - 1)
                    {
                        string twoCharStringToSample = name.Substring(lnJ, 1) + name.Substring(lnJ + 1, 1);
                        int twoCharIndex = Array.IndexOf(twoCharStrings, twoCharStringToSample);
                        if (twoCharIndex != -1)
                        {
                            byteArray.Add(0x22 + twoCharIndex);
                            lnJ++;
                            continue;
                        } else
                        {
                            char character = Convert.ToChar(name.Substring(lnJ, 1).ToLower());
                            byteArray.Add(character - 97);
                        }
                    } else
                    {
                        char character = Convert.ToChar(name.Substring(lnJ, 1).ToLower());
                        byteArray.Add(character - 97);
                    }
                }
                romData[0x2fba0 + stringMarker] = (byte)byteArray.ToArray().Length;
                stringMarker++;
                foreach(int byteSingle in byteArray)
                {
                    romData[0x2fba0 + stringMarker] = (byte)byteSingle;
                    stringMarker++;
                }
            }

            if (stringMarker > 86)
            {
                // bad.
                MessageBox.Show("Cannot continue hack; the names are too long to translate to rom hexadecimal.");
                return false;
            }

            romData[0x2fba0 + stringMarker + 0] = 4;
            romData[0x2fba0 + stringMarker + 1] = 0x16;
            romData[0x2fba0 + stringMarker + 2] = 0x32;
            romData[0x2fba0 + stringMarker + 3] = 0xa4;
            romData[0x2fba0 + stringMarker + 4] = 0xe5;
            romData[0x2fba0 + stringMarker + 5] = 3;
            romData[0x2fba0 + stringMarker + 6] = 0x26;
            romData[0x2fba0 + stringMarker + 7] = 0x3a;
            romData[0x2fba0 + stringMarker + 8] = 0x11;
            romData[0x2fba0 + stringMarker + 9] = 4;
            romData[0x2fba0 + stringMarker + 10] = 0x71;
            romData[0x2fba0 + stringMarker + 11] = 0x68;
            romData[0x2fba0 + stringMarker + 12] = 0x76;
            romData[0x2fba0 + stringMarker + 13] = 0x12;
            romData[0x2fba0 + stringMarker + 14] = 4;
            romData[0x2fba0 + stringMarker + 15] = 0x31;
            romData[0x2fba0 + stringMarker + 16] = 0xe4;
            romData[0x2fba0 + stringMarker + 17] = 0x38;
            romData[0x2fba0 + stringMarker + 18] = 0x12;
            // Calculate difference from 85 and stringMarker.
            int difference = 87 - stringMarker;
            romData[0x2fba0 + stringMarker + 19] = 1;
            romData[0x2fba0 + stringMarker + 20] = 0;
            romData[0x2fba0 + stringMarker + 21] = 1;
            romData[0x2fba0 + stringMarker + 22] = 0;
            romData[0x2fba0 + stringMarker + 23] = 1;
            romData[0x2fba0 + stringMarker + 24] = 0;
            romData[0x2fba0 + stringMarker + 25] = 1;
            romData[0x2fba0 + stringMarker + 26] = 0;
            romData[0x2fba0 + stringMarker + 27] = 1;
            romData[0x2fba0 + stringMarker + 28] = 0;
            romData[0x2fba0 + stringMarker + 29] = (byte)difference;
            for (int lnI = 0; lnI < difference; lnI++)
            {
                romData[0x2fba0 + stringMarker + 30 + lnI] = 0;
            }

            return true;
        }

        private void randomizeHeroStats(Random r1)
        {
            // Max array:  [7, 5]
            int[,] heroL41Gains = new int[,] { 
                { 156, 73, 155, 55, 82, 186 },
                { 46, 112, 119, 70, 118, 151 },
                { 69, 57, 75, 88, 109, 151 },
                { 27, 123, 105, 120, 169, 241 },
                { 24, 131, 97, 118, 164, 208 },
                { 93, 71, 121, 53, 46, 0 },
                { 194, 28, 229, 22, 36, 0 },
                { 195, 212, 170, 28, 164, 0 }
            };

            //heroL41Gains[8, 0] = 0;
            // Randomize the four multipliers from 8 to 32.  Each multiplier has six bytes.
            for (int lnI = 0; lnI < 4; lnI++)
                for (int lnJ = 0; lnJ < 6; lnJ++)
                {
                    int byteToUse2 = 0x4a281 + (lnI * 6) + lnJ;
                    romData[byteToUse2] = (byte)(((r1.Next() % 4) + 1) * 8);
                }

            // Randomize the levels to the next multiplier from 0 to 24.(First 4 bytes)  Always make the 5th byte "99" (63 hex).
            // Calculate the base gain based on the four multipliers.  Try to get as close to the target gain for each stat as possible.
            // Char byteToUse - 0x4a15b, 0x4a17f, 0x4a1a3, 0x4a1c7, 0x4a1eb, 0x4a20f, 0x4a22d, 0x4a24b
            int byteToUse = 0x4a15b;
            // 48 bytes for strength, 48 bytes for agility, 48 bytes for intelligence, 48 bytes for luck, 30 bytes for mp, in that order.  NOT in character order, statistic order!
            for (int lnJ = 0; lnJ < 6; lnJ++)
            {
                for (int lnI = 0; lnI < 8; lnI++)
                {
                    // Do NOT run this randomization for the MP calculations of Taloon, Ragnar, and Alena.
                    if (lnJ == 5 && lnI >= 5)
                        continue;

                    if (optMonsterSilly.Checked || optMonsterMedium.Checked)
                    {
                        int randomDir = (r1.Next() % 3);
                        int difference = heroL41Gains[lnI, lnJ] / (optMonsterSilly.Checked ? 4 : 2);
                        if (randomDir == 0)
                            heroL41Gains[lnI, lnJ] -= (r1.Next() % difference);
                        if (randomDir == 1)
                            heroL41Gains[lnI, lnJ] += (r1.Next() % difference);
                    }
                    if (optMonsterHeavy.Checked)
                        heroL41Gains[lnI, lnJ] = (r1.Next() % (lnJ == 3 ? 175 : 225)) + (lnJ == 3 ? 75 : 25);

                    int baseStat = 0;
                    for (int lnK = 0; lnK < 4; lnK++)
                    {
                        if (romData[byteToUse + lnK] >= 128)
                            baseStat += (lnK == 0 ? 1 : lnK == 1 ? 2 : lnK == 2 ? 4 : lnK == 3 ? 8 : 16);
                    }
                    if (optMonsterSilly.Checked || optMonsterMedium.Checked)
                    {
                        int randomDir = (r1.Next() % 3);
                        int difference = baseStat / (optMonsterSilly.Checked ? 2 : 1);
                        if (randomDir == 0 && difference >= 2)
                            baseStat -= (r1.Next() % difference);
                        if (randomDir == 1 && difference >= 2)
                            baseStat += (r1.Next() % difference);
                    }
                    if (optMonsterHeavy.Checked)
                        baseStat = r1.Next() % 32;

                    int[] levels = { 0, 0, 0, 0 };
                    for (int lnK = 0; lnK < 4; lnK++)
                        levels[lnK] = (byte)(r1.Next() % 50);
                    Array.Sort(levels);
                    for (int lnK = 0; lnK < 4; lnK++)
                    {
                        if ((lnK == 0 && baseStat % 2 == 1) || (lnK == 1 && baseStat % 4 >= 2) || (lnK == 2 && baseStat % 8 >= 4) || (lnK == 3 && baseStat % 16 >= 8))
                            romData[byteToUse + lnK] = (byte)(128 + levels[lnK]);
                        else
                            romData[byteToUse + lnK] = (byte)(levels[lnK]);
                    }

                    if (baseStat >= 16)
                        romData[byteToUse + 4] = 99 + 128;
                    else
                        romData[byteToUse + 4] = 99;

                    // Averages:  8-16 = .6/level, 24-32 = 1.6/level, 40-48 = 2.6/level, 56-64 = 3.6/level, 72-80 = 4.6/level, 88-96 = 5.6/level, 104-112 = 6.6/level
                    // Maximize base stat at 12 (5.6/level at 8 multiplier)
                    // Now to figure out the multiplier to use (+ 0) and the base multiplier (+ 5)
                    double[] diffs = { 0.0, 0.0, 0.0, 0.0 };
                    int[] baseMult = { 0, 0, 0, 0 };
                    for (int lnK = 0; lnK < 4; lnK++)
                    {
                        for (baseMult[lnK] = 1; baseMult[lnK] <= 12; baseMult[lnK]++)
                        {
                            int byteToUse2 = 0x4a281 + (lnK * 6); // multipliers
                            double stat = 0.0;
                            int multLevel = 0;

                            for (int lnL = 2; lnL <= 40; lnL++)
                            {
                                if (lnL > (romData[byteToUse + multLevel] % 128))
                                    multLevel++;
                                stat += Math.Floor((((double)baseMult[lnK] * romData[byteToUse2 + multLevel]) - 8) / 16) + 0.6;
                            }
                            //baseMult[lnK] = (int)Math.Round(heroL41Gains[lnI, lnJ] / stat);
                            diffs[lnK] = Math.Abs(stat - heroL41Gains[lnI, lnJ]);
                            if (stat > heroL41Gains[lnI, lnJ]) break;
                        }
                    }

                    double lowDiff = 9999;
                    int lowMult = 0;
                    int ultiBaseMult = 0;
                    for (int lnK = 0; lnK < 4; lnK++)
                    {
                        if (diffs[lnK] < lowDiff)
                        {
                            lowDiff = diffs[lnK];
                            lowMult = lnK;
                            ultiBaseMult = baseMult[lnK];
                        }
                    }
                    romData[byteToUse] += (byte)(32 * lowMult);
                    romData[byteToUse + 5] = (byte)ultiBaseMult;

                    byteToUse += 6;
                }
            }
            //int asdf = 1234;
            //overrideStats();
        }

        //private void overrideStats()
        //{
        //    for (int lnI = 0; lnI < 4; lnI++)
        //        for (int lnJ = 0; lnJ < 6; lnJ++)
        //        {
        //            int byteToUse2 = 0x4a281 + (lnI * 6) + lnJ;
        //            romData[byteToUse2] = 24;
        //        }

        //    int byteToUse = 0x4a15b;
        //    // 48 bytes for strength, 48 bytes for agility, 48 bytes for intelligence, 48 bytes for luck, 30 bytes for mp, in that order.  NOT in character order, statistic order!
        //    for (int lnJ = 0; lnJ < 6; lnJ++)
        //    {
        //        for (int lnI = 0; lnI < 8; lnI++)
        //        {
        //            // Do NOT run this randomization for the MP calculations of Taloon, Ragnar, and Alena.
        //            if (lnJ == 5 && lnI >= 5)
        //                continue;

        //            romData[byteToUse + 0] = 144;
        //            romData[byteToUse + 1] = 16;
        //            romData[byteToUse + 2] = 16;
        //            romData[byteToUse + 3] = 16;
        //            romData[byteToUse + 4] = 99;
        //            romData[byteToUse + 5] = 1;
        //            byteToUse += 6;
        //        }
        //    }
        //}

        private void randomizeMonsterAttacks(Random r1)
        {
            int[] level1Moves = { 0x00, 0x07, 0x16, 0x1c, 0x1e, 0x25, 0x2a, 0x2d, 0x30, 0x32, 0x32, 0x32, 0x32, 0x32, 0x36, 0x47, 0x48 };
            int[] level2Moves = { 0x00, 0x03, 0x07, 0x0a, 0x10, 0x12, 0x13, 0x16, 0x17, 0x1c, 0x1d, 0x1e, 0x22, 0x25, 0x2a, 0x2d, 0x30, 0x32, 0x32, 0x32, 0x32, 0x34, 0x35, 0x36, 0x38, 0x3c, 0x3f, 0x42, 0x43, 0x47, 0x48, 0x4b, 0x4c, 0x57, 0x58, 0x5f, 0x64 };
            int[] level3Moves = { 0x00, 0x01, 0x03, 0x04, 0x07, 0x08, 0x0a, 0x0b, 0x0d, 0x10, 0x11, 0x12, 0x13, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f, 0x25, 0x26, 0x2d, 0x2e, 0x30, 0x32, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3c, 0x3d, 0x3f, 0x40, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4d, 0x4e, 0x4f, 0x56, 0x58, 0x5d, 0x5e, 0x62, 0x63, 0x66 };
            int[] level4Moves = { 0x01, 0x02, 0x04, 0x05, 0x08, 0x09, 0x0b, 0x0c, 0x0d, 0x0e, 0x10, 0x14, 0x17, 0x1a, 0x1d, 0x1f, 0x20, 0x21, 0x23, 0x24, 0x26, 0x27, 0x28, 0x2e, 0x31, 0x32, 0x34, 0x37, 0x39, 0x3a, 0x3d, 0x3e, 0x40, 0x41, 0x42, 0x44, 0x45, 0x49, 0x4a, 0x4d, 0x50, 0x51, 0x53, 0x54, 0x56, 0x58, 0x59, 0x5a, 0x5d, 0x60, 0x62, 0x63 };
            int[] level5Moves = { 0x02, 0x05, 0x06, 0x09, 0x0c, 0x0e, 0x10, 0x17, 0x1a, 0x1d, 0x24, 0x27, 0x28, 0x29, 0x31, 0x32, 0x34, 0x37, 0x3a, 0x3e, 0x41, 0x44, 0x49, 0x4a, 0x52, 0x53, 0x56, 0x59, 0x5a, 0x60 };
            int[] weirdAttackMoves = { 0x30, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };

            for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
            {
                int byteToUse = 0x60056 + (monsterRank[lnI] * 22);
                int randomType = 0;
                int randomRandom = (r1.Next() % 4);
                if (optMonsterLight.Checked)
                    randomType = (randomRandom == 0 || randomRandom == 1 ? 0 : randomRandom == 2 ? 1 : 2);
                if (optMonsterSilly.Checked)
                    randomType = randomRandom;
                if (optMonsterMedium.Checked)
                    randomType = (randomRandom == 0 ? 1 : randomRandom == 1 ? 2 : 3);
                if (optMonsterHeavy.Checked)
                    randomType = 3;

                // Force early chapter bosses to have no worse than level 2 moves. (part 1)
                if (randomType == 3 && (monsterRank[lnI] == 0xb3 || monsterRank[lnI] == 0x12 || monsterRank[lnI] == 0xaf ||
                    monsterRank[lnI] == 0xb0 || monsterRank[lnI] == 0xb1 || monsterRank[lnI] == 0xb2 || monsterRank[lnI] == 0xba || monsterRank[lnI] == 0xb4))
                    randomType = 2;

                if (randomType == 0)
                    continue;
                if (randomType == 1 && monsterRank[lnI] != 0x5c && monsterRank[lnI] != 0x75)
                {
                    if (r1.Next() % 2 == 1)
                        // weird attack pattern
                        for (int lnJ = 0; lnJ < 6; lnJ++)
                        {
                            romData[byteToUse + 9 + lnJ] = (byte)weirdAttackMoves[r1.Next() % weirdAttackMoves.Length];
                            // Chapter 1-4 Bosses should not critical hit or paralyze
                            if ((romData[byteToUse + 9 + lnJ] == 0x33 || romData[byteToUse + 9 + lnJ] == 0x34 || romData[byteToUse + 9 + lnJ] == 0x37) &&
                                (monsterRank[lnI] == 0xb3 || monsterRank[lnI] == 0x12 || monsterRank[lnI] == 0xaf || monsterRank[lnI] == 0xb0 ||
                                  monsterRank[lnI] == 0xb1 || monsterRank[lnI] == 0xb2 || monsterRank[lnI] == 0xba || monsterRank[lnI] == 0xb4))
                                lnJ--;
                        }
                    else
                        for (int lnJ = 0; lnJ < 6; lnJ++)
                            romData[byteToUse + 9 + lnJ] = 0x32;
                }
                if (randomType == 2)
                {
                    int moveLevel = (lnI < 38 ? 0 : lnI < 76 ? 1 : lnI < 114 ? 2 : lnI < 152 ? 3 : 4);
                    // Force early chapter bosses to have no worse than level 2 moves. (part 2)
                    if (monsterRank[lnI] == 0xb3 || monsterRank[lnI] == 0x12 || monsterRank[lnI] == 0xaf || monsterRank[lnI] == 0xb0 ||
                        monsterRank[lnI] == 0xb1 || monsterRank[lnI] == 0xb2 || monsterRank[lnI] == 0xba || monsterRank[lnI] == 0xb4)
                        moveLevel = 1;

                    for (int lnJ = 0; lnJ < 6; lnJ++)
                    {
                        if (lnJ >= 0 && lnJ <= 1 && (monsterRank[lnI] == 0x5c || monsterRank[lnI] == 0x75))
                            romData[byteToUse + 9 + lnJ] = 0x47;
                        else
                            romData[byteToUse + 9 + lnJ] = (byte)(moveLevel == 0 ? level1Moves[r1.Next() % level1Moves.Length] :
                                                                  moveLevel == 1 ? level2Moves[r1.Next() % level2Moves.Length] :
                                                                  moveLevel == 2 ? level3Moves[r1.Next() % level3Moves.Length] :
                                                                  moveLevel == 3 ? level4Moves[r1.Next() % level4Moves.Length] :
                                                                  level5Moves[r1.Next() % level5Moves.Length]);

                        // Chapter 1-4 Boss monsters should not have a chance to crit nor be able to call for more bosses.  If they come up, redo the randomization.
                        if ((romData[byteToUse + 9 + lnJ] == 0x48 || romData[byteToUse + 9 + lnJ] == 0x34) && ((monsterRank[lnI] == 0xb3 || monsterRank[lnI] == 0x12 || monsterRank[lnI] == 0xaf ||
                            monsterRank[lnI] == 0xb0 || monsterRank[lnI] == 0xb1 || monsterRank[lnI] == 0xb2 || monsterRank[lnI] == 0xba || monsterRank[lnI] == 0xb4)))
                        {
                            lnJ--;
                        }
                    }
                }
                if (randomType == 3)
                {
                    for (int lnJ = 0; lnJ < 6; lnJ++)
                    {
                        if (lnJ >= 0 && lnJ <= 1 && (monsterRank[lnI] == 0x5c || monsterRank[lnI] == 0x75))
                            romData[byteToUse + 9 + lnJ] = 0x47;
                        else if (r1.Next() % 2 == 1)
                            romData[byteToUse + 9 + lnJ] = (byte)(r1.Next() % 0x67);
                        else
                            romData[byteToUse + 9 + lnJ] = 0x32;
                        if (romData[byteToUse + 9 + lnJ] == 0x15 || romData[byteToUse + 9 + lnJ] == 0x55)
                            lnJ--; // redo randomization.  Transform and super slime is a bad idea to use here.
                    }
                    if (lnI == 0x42 || lnI == 0x58)
                    {

                    }
                }

                byte pct25Attack = 0x00;
                for (int lnJ = 0; lnJ < 6; lnJ++)
                {
                    if (romData[byteToUse + 9 + lnJ] == 0x0f || romData[byteToUse + 9 + lnJ] == 0x21 || romData[byteToUse + 9 + lnJ] == 0x29 || romData[byteToUse + 9 + lnJ] == 0x58 ||
                        romData[byteToUse + 9 + lnJ] == 0x59 || romData[byteToUse + 9 + lnJ] == 0x5a)
                    {
                        pct25Attack = romData[byteToUse + 9 + lnJ];
                        break;
                    }
                }
                if (pct25Attack != 0x00)
                {
                    // Force pattern change at 25% HP.
                    romData[byteToUse + 9 + 0] = pct25Attack;
                    romData[byteToUse + 9 + 1] = (byte)(pct25Attack + 128);

                    for (int lnJ = 2; lnJ < 6; lnJ++)
                    {
                        if (romData[byteToUse + 9 + lnJ] == 0x0f || romData[byteToUse + 9 + lnJ] == 0x21 || romData[byteToUse + 9 + lnJ] == 0x29 || romData[byteToUse + 9 + lnJ] == 0x58 ||
                            romData[byteToUse + 9 + lnJ] == 0x59 || romData[byteToUse + 9 + lnJ] == 0x5a)
                        {
                            romData[byteToUse + 9 + lnJ] = 0x32; // change mutation pattern to basic attack
                        }
                    }
                }
            }
        }

        private void randomizeMonsterStats(Random r1)
        {

            // If light is selected, 50% chance of maintaining same enemy pattern.  25% chance of changing it to attack only.  25% chance of the level moves above.
            // If silly is selected, 25% chance of maintaining same enemy pattern, 25% chance of attack only, 25% chance of level moves above, 25% of completely random.
            // If ridiculous is selected, 25% chance of attack only, 25% chance of level moves above, 50% chance of completely random.
            // If ludicrous is selected, 100% chance of completely random.

            for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
            {
                //// do not randomize Necrosaro.
                //if (lnI == 0xae) continue;

                int byteToUse = 0x60056 + (monsterRank[lnI] * 22);

                // If light is selected, maintain enemy resistances, HP, strength, defense, and agility.  Do not do this if a metal monster is involved...
                if (!optMonsterLight.Checked && monsterRank[lnI] != 0x5c && monsterRank[lnI] != 0x75)
                {
                    // If silly is selected, adjust HP, strength, defense, and agility by +/- 25%.
                    // If ridiculous is selected, adjust HP, strength, defense, and agility by +/- 50%.
                    // If ludicrous is selected, adjust HP, strength, defense, and agility by +/- 100%.
                    for (int lnJ = 2; lnJ <= 6; lnJ++)
                    {
                        if (lnJ == 3) lnJ++;
                        int stat = romData[byteToUse + lnJ] + (lnJ >= 4 ? (romData[byteToUse + lnJ + 11] % 4) * 256 : 0);
                        int randomModifier = (r1.Next() % 3);
                        try
                        {
                            if (randomModifier == 0)
                                stat -= (r1.Next() % (stat / (optMonsterSilly.Checked ? 4 : optMonsterMedium.Checked ? 2 : 1)));
                            else if (randomModifier == 2)
                                stat += (r1.Next() % (stat / (optMonsterSilly.Checked ? 4 : optMonsterMedium.Checked ? 2 : 1)));
                        }
                        catch (DivideByZeroException)
                        {
                            // skip error and do not adjust the stat.  Crash on all other exceptions.
                        }

                        if (lnJ == 2 && stat > 255) stat = 255;
                        if (lnJ >= 4 && stat > 1020) stat = 1020;
                        romData[byteToUse + lnJ] = (byte)(stat % 256);
                        if (lnJ >= 4)
                            romData[byteToUse + lnJ + 11] = (byte)(romData[byteToUse + lnJ + 11] - (romData[byteToUse + lnJ + 11] % 4) + (stat / 256));
                    }

                    // If ludicrous is selected, change chances of double and triple attacks.
                    // But do not allow this if it's an early chapter boss monster.
                    if (optMonsterHeavy.Checked && !(monsterRank[lnI] == 0xb3 || monsterRank[lnI] == 0x12 || monsterRank[lnI] == 0xaf || monsterRank[lnI] == 0xb0 ||
                        monsterRank[lnI] == 0xb1 || monsterRank[lnI] == 0xb2 || monsterRank[lnI] == 0xba || monsterRank[lnI] == 0xb4))
                    {
                        romData[byteToUse + 11] = (byte)(romData[byteToUse + 11] % 128);
                        romData[byteToUse + 12] = (byte)(romData[byteToUse + 12] % 128);

                        if (r1.Next() % 1000 <= lnI)
                        {
                            romData[byteToUse + 11] += 128;
                            romData[byteToUse + 12] += 128;
                        }
                        else if (r1.Next() % 1000 <= lnI)
                        {
                            romData[byteToUse + 12] += 128;
                        }
                        else if (r1.Next() % 1000 <= lnI)
                        {
                            romData[byteToUse + 11] += 128;
                        }

                        // If ludicrous is selected, change chances of regeneration.
                        romData[byteToUse + 13] = (byte)(romData[byteToUse + 13] % 128);
                        romData[byteToUse + 14] = (byte)(romData[byteToUse + 14] % 128);

                        if (r1.Next() % 1000 <= lnI)
                        {
                            romData[byteToUse + 13] += 128;
                            romData[byteToUse + 14] += 128;
                        }
                        else if (r1.Next() % 1000 <= lnI)
                        {
                            romData[byteToUse + 14] += 128;
                        }
                        else if (r1.Next() % 1000 <= lnI)
                        {
                            romData[byteToUse + 13] += 128;
                        }
                    }
                }
            }
        }

        private void randomizeTreasures(Random r1)
        {
            int[] c1p1Treasure = { 0x7bd1d, // Burland
                    0x7bf38, 0x7bf37, // Cave To Izmit
                    0x7bd6a, // Izmit
                    0x7bf15, 0x7bf16, 0x7bf17, 0x7bdb7, 0x7b936 }; // Old Well - Flying Shoes - 9
            int[] c1p2Treasure = { 0x7bf47, 0x7bf48, 0x7bf49, 0x7bf4a, 0x7bf4b, 0x7bf4c }; // Loch Tower - End of C1 - 6 (15)
            int[] c2p1Treasure = { 0x7bd0f, 0x7bd16, // Santeem
                    0x7bdc7, // Tempe
                    0x7bf10, 0x7bf11, 0x7bf12, 0x7bf13, 0x7bf14, // Frenor 
                    0x7bd4e, 0x7bd55 }; //  Bazaar - Thief's Key - 10 (25)
            int[] c2p2Treasure = { 0x7bd08, // Santeem (Thief's Key)
                    0x7bf41, 0x7bf42, 0x7bf43, 0x7b901 }; // Birdsong Tower - Birdsong Nectar(0x7b8f4?) - 5 (30)
            int[] c3p1Treasure = {  0x7bd86, // Lakanaba
                    0x7bf2a, 0x7bf2b, // Iron Safe Cave
                    0x560e8, // Foxville fox
                    0x7bf2d, 0x7bf2e, 0x7bf2f, 0x7bf30, 0x7bf31, 0x7bf32, 0x7bf33, 0x7bf34, 0x7bf35, 0x7bf36 }; // Silver Statuette Cave - Silver Statuette - 14 (44)
            int[] c4p1Treasure = { 0x7bd7f, 0x7bd78, 0x7bdca, // Monbaraba
                    0x7bd8d, 0x7beee, // Kievs
                    0x7bf0a, 0x7bf0b, 0x7bf0c, 0x7ba05, 0x7bf0e, // Cave West of Kievs (couple 0x7ba05 with 0x7ba0a)
                    0x7bef0, 0x7bef1, 0x7bef2 }; // Aktemto Mine - Gunpowder Jar and Sphere Of Silence - 13 (57)
            int[] c5p1Treasure = { 0x7bd71, 0x7bdc9, // Hometown
                    0x7b96a, 0x7b983, // Woodman's Shack
                    //0x7bf2c, // Cave Of Betrayal - Can't change this due to the way the cave works
                    0x7beef }; // Desert Inn - Symbol Of Faith - 5 (62)
            int[] c5p2Treasure = { 0x7bdc8, // Aneaux
                    0x7bf4d, 0x7bf4e, 0x7bf4f, 0x7bf50, 0x7bf51, 0x7bf52, 0x7bf53, 0x7bf54, 0x7bf55 }; // Great Lighthouse - Fire Of Serenity - 10 (72)
            int[] c5p3Treasure = { 0x7bdc6, // Mintos
                    0x7bdcc, // Shrine East Of Mintos
                    0x7befe, 0x7beff, 0x7bf00, 0x7bf01, 0x7bf02, 0x7bf03, // Cave Of The Padequia
                    0x7bda9, // Old Man's Island House
                    0x7bdcb, 0x7b94b, // Seaside Village
                    0x7bd40, 0x7bd47, // Stancia Castle
                    0x7bdc5 }; // Riverton - Padequia Root - 14 (86)
            int[] c5p4Treasure = { 0x7bdb0, 0x7bf0f }; // Cave West Of Kievs - Pre-Magic Key - 2 (88)
            int[] c5p5Treasure = { 0x7becd, 0x7bece, 0x7becf, 0x7bed0, 0x7bed1, 0x7bed2, // Burland Castle
                    0x7beca, 0x7becb, 0x7becc, // Santeem Castle
                    0x7beda, 0x7bedb, 0x7bedd, 0x7bede, 0x7bd2b, // Endor
                    0x7befb, 0x7befc, 0x7befd }; // Shrine Of Breaking Waves - Pre-Magma Staff - 17 (105)
            int[] c5p6Treasure = { 0x7bd32, 0x7bd39, 0x7bee7, // Gardenbur Castle
                    0x7bf04, 0x7bf05, 0x7bf06, 0x7bf07, 0x7bf08, 0x7bf09 }; // Cave SE Of Gardenbur - Pre-Final Key - 9 (114)
            int[] c5p7Treasure = { 0x7beeb, 0x7beec, 0x7beed, // Lakanaba
                    0x7bee3, 0x7bee4, 0x7bee5, // Branca Castle
                    0x7bf56, // Konenber
                    0x7bee6, // Gardenbur Castle
                    0x7bf39, 0x7bf3a, 0x7bf3b, // Royal Crypt
                    0x7bd63, 0x7bd5c, // Haville
                    0x7bf64, 0x7bf65, 0x7bf66, 0x7bf67, 0x7bf68 }; // Colossus - Pre-Staff Of Transform - 18 (132)
            int[] c5p8Treasure = { 0x7bd24, 0x7bdc3, 0x7bed3, 0x7bed4, 0x7bed5, 0x7bed6, // Dire Palace
                    0x7bdc4, 0x7bef3, 0x7bef4, 0x7bef5, 0x7bef6, 0x7bef7, 0x7bef8, 0x7bef9, 0x7befa }; // Aktemto Bonus Round - Gas Canister & Stone Of Drought - 15 (147)
            int[] c5p9Treasure = { 0x7bf44, 0x7bf45, 0x7bf46, // World Tree
                    0x7bd94, // Gottside
                    0x7bf18, 0x7bf19, 0x7bf1a, 0x7bf1b, 0x7bf1c, // Cascade Cave
                    0x7bf61, 0x7bf63 }; // Shrine Of Horn - All Zenithian equipment - 11 (158)
            int[] c5p10Treasure = { 0x7bf3c, 0x7bf3d, 0x7bf3e, 0x7bf3f, 0x7bf40, // Zenithian Tower
                    0x7bd01, // Zenithian Castle
                    0x7bf1d, 0x7bf1e, 0x7bf1f, 0x7bf20, 0x7bf21, 0x7bf22, 0x7bf23, 0x7bf24, 0x7bf25, 0x7bf26, 0x7bf27, 0x7bf28, 0x7bf29, // Final Cave
                    0x7bdcd, // Gigademon Area
                    0x7bf5c, // Radimvice Area
                    0x7bf5d, 0x7bf5d, 0x7bf5d, 0x7bf60 }; // Necrosaro's Palace - Baron's Horn & End Of Game - 25 (183)
            int[] c5DeadZone = { 0x7bda2, 0x7bd9b, // Konenber ships -> lost forevers,
                                 0x7bedc }; // Endor (Chapter 2/3) - 3 (186)

            List<int> allTreasureList = new List<int>();
            allTreasureList = addTreasure(allTreasureList, c1p1Treasure);
            allTreasureList = addTreasure(allTreasureList, c1p2Treasure);
            allTreasureList = addTreasure(allTreasureList, c2p1Treasure);
            allTreasureList = addTreasure(allTreasureList, c2p2Treasure);
            allTreasureList = addTreasure(allTreasureList, c3p1Treasure);
            allTreasureList = addTreasure(allTreasureList, c4p1Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p1Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p2Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p3Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p4Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p5Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p6Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p7Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p8Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p9Treasure);
            allTreasureList = addTreasure(allTreasureList, c5p10Treasure);
            allTreasureList = addTreasure(allTreasureList, c5DeadZone);

            int[] allTreasure = allTreasureList.ToArray();

            // 33 percent chance of a common treasure.
            // 33 percent chance after that for gold.
            // After that, present the rest of the items, including man eater chest and mimics.

            int[] legalTreasures = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
                                         0x10, 0x11, 0x12, 0x13, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1f,
                                         0x20, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
                                         0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,
                                         0x40, 0x41, 0x42, 0x43, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4c, 0x4d, 0x4e, 0x4f,
                                         0x50, 0x57, 0x59, 0x5b,
                                         0x66, 0x69,
                                         0x79, 0xfd, 0xfe };
            int[] legalTreasures2 = { 0x53, 0x54, 0x55, 0x56, 0x58, 0x5a, 0x5e, 0x61, 0x62, 0x63, 0x64, 0x65, 0x74 };


            // Completely randomize treasures first.
            foreach (int treasure in allTreasureList)
            {
                if (r1.Next() % 3 == 0)
                    romData[treasure] = (byte)legalTreasures2[r1.Next() % legalTreasures2.Length];
                else if (r1.Next() % 3 == 0 && treasure >= 0x7bd01 && treasure <= 0x7bfff)
                    romData[treasure] = (byte)(r1.Next() % 124 + 128);
                else
                    romData[treasure] = (byte)legalTreasures[r1.Next() % legalTreasures.Length];
                //romData[treasure] = (byte)legalTreasures[r1.Next() % legalTreasures.Length];
                if (treasure == 0x7b901)
                    romData[0x7b8f4] = romData[treasure];
            }

            // Then assign key items, overwriting the randomized treasures.
            int[] keyItems = { 0x6c,
                    0x76, 0x75,
                    0x6b, 0x6d,
                    0x5d, 0x70,
                    0x7c, 0x7b, 0x72, 0x1e, 0x68, 0x5c, 0x7d, 0x14, 0x37, 0x44, 0x4b, 0x52,
                    0x60, 0x67, 0x6e, 0x5f, 0x6a };
            List<int> keyItemList = new List<int> { };
            addTreasure(keyItemList, keyItems);

            int[] minItemZones = { 0,
                    15, 15,
                    30, 30,
                    44, 44,
                    57, 57, 57, 57, 57, 57, 72, 72, 72, 72, 72, 72,
                    0, 0, 0, 0, 72 };
            int[] maxItemZones = { 9,
                    25, 30,
                    44, 44,
                    57, 57,
                    72, 86, 88, 105, 132, 147, 147, 158, 158, 158, 158, 183,
                    183, 183, 183, 183, 147 };

            for (int lnJ = 0; lnJ < keyItems.Length; lnJ++)
            {
                int treasureLocation = allTreasure[minItemZones[lnJ] + (r1.Next() % (maxItemZones[lnJ] - minItemZones[lnJ]))];
                if (keyItemList.Contains(romData[treasureLocation]))
                {
                    lnJ--;
                    continue;
                }
                romData[treasureLocation] = (byte)keyItems[lnJ];
                if (treasureLocation == 0x7b901)
                    romData[0x7b8f4] = (byte)keyItems[lnJ];
            }

            using (StreamWriter writer = File.CreateText(Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW4TreasureOutput.txt")))
            {
                treasureOutput(writer, "c1p1", c1p1Treasure);
                treasureOutput(writer, "c1p2", c1p2Treasure);
                treasureOutput(writer, "c2p1", c2p1Treasure);
                treasureOutput(writer, "c2p2", c2p2Treasure);
                treasureOutput(writer, "c3p1", c3p1Treasure);
                treasureOutput(writer, "c4p1", c4p1Treasure);
                treasureOutput(writer, "c5p1", c5p1Treasure);
                treasureOutput(writer, "c5p2", c5p2Treasure);
                treasureOutput(writer, "c5p3", c5p3Treasure);
                treasureOutput(writer, "c5p4", c5p4Treasure);
                treasureOutput(writer, "c5p5", c5p5Treasure);
                treasureOutput(writer, "c5p6", c5p6Treasure);
                treasureOutput(writer, "c5p7", c5p7Treasure);
                treasureOutput(writer, "c5p8", c5p8Treasure);
                treasureOutput(writer, "c5p9", c5p9Treasure);
                treasureOutput(writer, "c5p10", c5p10Treasure);
                treasureOutput(writer, "c5dead", c5DeadZone);
            }
        }

        private void randomizeStores(Random r1)
        {
            // We first need to adjust the prices of all the items so that the casino and medal king stores sell at a proper price.
            int[] prices = { 10, 30, 100, 1500, 550, 880, 2000, 5500, 500, 200, 1250, 350, 1600, 620, 50000, 1400,
                             2500, 20000, 600, 3300, 0, 50000, 7500, 4300, 8000, 750, 10000, 20000, 30000, 4000, 0, 4000,
                             6000, 0, 4000, 3500, 10, 70, 180, 350, 1200, 1500, 2300, 110, 400, 700, 15000, 600,
                             250, 6300, 5200, 3000, 4400, 15000, 7500, 0, 9800, 6000, 870, 8800, 1000, 90, 180, 650,
                             13000, 9000, 4700, 7100, 0, 50000, 65, 120, 1100, 3500, 280, 0, 8, 540, 15000, 50000,
                             5000, 1000, 0, 8, 10, 20, 25, 10000, 30, 6000, 500, 10000, 0, 0, 150, 0,
                             0, 2000, 1500, 700, 2000, 3000, 5, 5, 0, 4000, 0, 0, 0, 0, 1000, 0,
                             0, 0, 0, 0, 10, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0 };

            // Go through the prices and plug into the ROM.  If the price is 0, make sure that 0x10 is set in the first segment.
            for (int lnI = 0; lnI < prices.Length; lnI++)
            {
                int oldValue = romData[0x40cf4 + lnI];
                bool noThrow = (oldValue % 32 >= 16);
                if (noThrow && prices[lnI] >= 1)
                    oldValue -= 16;
                oldValue -= (oldValue % 4);
                oldValue += (prices[lnI] >= 10000 ? 3 : prices[lnI] >= 1000 ? 2 : prices[lnI] >= 100 ? 1 : 0);
                romData[0x40cf4 + lnI] = (byte)oldValue;

                romData[0x40df2 + lnI] = (byte)(prices[lnI] / (prices[lnI] >= 10000 ? 1000 : prices[lnI] >= 1000 ? 100 : prices[lnI] >= 100 ? 10 : 1));
            }

            int[] lowGradeStores = { 0x6341f, 0x634a1, 0x63537, 0x63425, 0x634a9, 0x6353C,
                    0x63496, 0x634b1, 0x6345a, 0x63541, 0x6342c, 0x63549, 0x63433, 0x634b9, 0x63551, 0x634c1, 0x63558, 0x6343a, 0x63560,
                    0x63441, 0x634c9, 0x63569, 0x6356f, 0x63573, 0x63446, 0x634e8, 0x635c8,
                    0x6344d, 0x634d1, 0X63579, 0x63453, 0x634d8, 0x63581,
                    0x63462, 0x634e0, 0x63590, 0x63468, 0x634f0, 0x63596, 0x63470, 0x634f8, 0x6359d }; // 42
            int[] highGradeStores = { 0x63564, 0x63477, 0x6348f, 0x63524, 0x635b3, 0x635ae, 0X63483, 0x63507, 0x635aa,
                    0x63530, 0x635ba, 0x6347f, 0x634ff, 0x635a4, 0x6350e, 0x63489, 0x63515, 0x6352c, 0x6351d, 0x635c0, 0x63588, 0x6349b}; // 22

            byte[] legalLowGradeStoreItems = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x19,
                    0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2b, 0x2c, 0x2d, 0x2f, 0x30,
                    0x3d, 0x3e, 0x3f, 0x46, 0x47, 0x4a, 0x53, 0x54, 0x55, 0x56, 0x58, 0x5a, 0x74 };
            byte[] legalHighGradeStoreItems = { 0x06, 0x07, 0x0c, 0x0e, 0x0f, 0x10, 0x11, 0x15, 0x16, 0x17, 0x18, 0x1a, 0x1b, 0x1c, 0x1d, 0x20, 0x22, 0x23,
                    0x2a, 0x2e, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x38, 0x39, 0x3b,
                    0x40, 0x41, 0x42, 0x43, 0x45, 0x48, 0x49, 0x4d, 0x4e, 0x4f,
                    0x53, 0x54, 0x55, 0x56, 0x58, 0x59, 0x5a, 0x5b, 0x5e, 0x61, 0x62, 0x63, 0x64, 0x65, 0x69, 0x74 };

            for (int lnI = 0; lnI < lowGradeStores.Length; lnI++)
            {
                List<int> store = new List<int> { };
                bool lastItem = false;
                int byteToUse = lowGradeStores[lnI];
                int lnJ = 0;
                do
                {
                    if (byteToUse == 0x63573 && lnJ == 0)
                    {
                        romData[byteToUse] = 0x56;
                        lnJ++;
                        continue;
                    }
                    if (romData[byteToUse + lnJ] >= 128)
                        lastItem = true;
                    romData[byteToUse + lnJ] = legalLowGradeStoreItems[r1.Next() % legalLowGradeStoreItems.Length];
                    bool failure = false;
                    for (int lnK = 0; lnK < lnJ; lnK++)
                        if (romData[byteToUse + lnJ] == romData[byteToUse + lnK])
                            failure = true;
                    if (lastItem)
                        romData[byteToUse + lnJ] += 128;
                    if (failure)
                    {
                        lastItem = false;
                        continue;
                    }
                    lnJ++;
                } while (!lastItem);
            }
            for (int lnI = 0; lnI < highGradeStores.Length; lnI++)
            {
                List<int> store = new List<int> { };
                bool lastItem = false;
                int byteToUse = highGradeStores[lnI];
                int lnJ = 0;
                do
                {
                    if (romData[byteToUse + lnJ] >= 128)
                        lastItem = true;
                    romData[byteToUse + lnJ] = legalHighGradeStoreItems[r1.Next() % legalHighGradeStoreItems.Length];
                    // Casino adjustment
                    if (byteToUse == 0x63588)
                    {
                        romData[0x5735d + (lnJ * 3)] = romData[byteToUse + lnJ];
                        int casinoCoins = prices[romData[byteToUse + lnJ]] / 20;
                        romData[0x5735d + (lnJ * 3) + 1] = (byte)(casinoCoins % 256);
                        romData[0x5735d + (lnJ * 3) + 2] = (byte)(casinoCoins / 256);
                    }

                    // Small Medal King adjustment
                    if (byteToUse == 0x6349b)
                    {
                        if (lnJ <= 2)
                        {
                            romData[0x5730b + (lnJ * 8)] = romData[byteToUse + lnJ];
                            romData[0x5730b + (lnJ * 8) + 4] = (byte)(prices[romData[byteToUse + lnJ]] / 4000);
                            if (romData[0x5730b + (lnJ * 8) + 4] == 0x00) romData[0x5730b + (lnJ * 8) + 4] = 0x01;
                        } else
                        {
                            romData[0x57323] = (byte)(prices[romData[byteToUse + lnJ]] / 4000);
                            if (romData[0x57323] == 0x00) romData[0x57323] = 0x01;
                        }
                    }

                    for (int lnK = 0; lnK < lnJ; lnK++)
                        if (romData[byteToUse + lnJ] == romData[byteToUse + lnK])
                            continue;
                    if (lastItem)
                        romData[byteToUse + lnJ] += 128;
                    lnJ++;
                } while (!lastItem);
            }
        }

        private void randomizeHeroEquipment(Random r1)
        {
            int[] minWeapon = { 255, 255, 255, 255, 255, 255, 255, 255 };
            int[] minArmor = { 255, 255, 255, 255, 255, 255, 255, 255 };
            int[] totalChances = { 35, 25, 9, 10 };
            int[,] equipChances = { { 17, 17, 7, 7 }, { 16, 11, 6, 6 }, { 17, 14, 4, 7 }, { 12, 12, 1, 5 }, { 10, 7, 3, 3 }, { 13, 9, 4, 5 }, { 19, 13, 7, 6 }, { 7, 12, 0, 5 } };

            for (int lnI = 0; lnI < 80; lnI++)
                romData[0x40c75 + lnI] = 0;

            for (int lnI = 0; lnI < 80; lnI++)
            {
                if (lnI == 0x21)
                    continue;
                // Totals - 35/25/9/10
                //if ((string)cboSoloHero.SelectedItem == "Hero") power = 0; - 18/18/8/8 - subtract 1 from each because of Zenithian equipment
                //if ((string)cboSoloHero.SelectedItem == "Cristo") power = 1; - 16/11/6/6
                //if ((string)cboSoloHero.SelectedItem == "Nara") power = 2; - 17/14/4/7
                //if ((string)cboSoloHero.SelectedItem == "Mara") power = 3; - 12/12/1/5
                //if ((string)cboSoloHero.SelectedItem == "Brey") power = 4; - 10/7/3/3
                //if ((string)cboSoloHero.SelectedItem == "Taloon") power = 5; - 13/9/4/5
                //if ((string)cboSoloHero.SelectedItem == "Ragnar") power = 6; - 19/13/7/6
                //if ((string)cboSoloHero.SelectedItem == "Alena") power = 7; - 7/12/0/5

                for (int lnJ = 0; lnJ < 8; lnJ++)
                {
                    int lnK = (lnI < 36 ? 0 : lnI < 61 ? 1 : lnI < 70 ? 2 : 3);
                    bool equippable = false;
                    // Hero must be able to equip the Zenithian equipment
                    if (lnJ == 0 && (lnI == 0x14 || lnI == 0x37 || lnI == 0x44 || lnI == 0x4b))
                        equippable = true;
                    else if (r1.Next() % totalChances[lnK] < (equipChances[lnJ, lnK] * (lnI == 0x00 || lnI == 0x01 || lnI == 0x02 ||
                                                                                        lnI == 0x24 || lnI == 0x25 || lnI == 0x26 ||
                                                                                        lnI == 0x3d || lnI == 0x3e ||
                                                                                        lnI == 0x46 || lnI == 0x47 ? 2 : 1)))
                        equippable = true;
                    else
                        equippable = false;

                    if (equippable)
                    {
                        romData[0x40c75 + lnI] += (byte)Math.Pow(2, lnJ);
                        // If you can equip the weak Zenithian Sword, you can equip the strong Zenithian Sword.
                        if (lnI == 0x14)
                            romData[0x40c75 + 0x21] += (byte)Math.Pow(2, lnJ);
                        if (minWeapon[lnJ] == 255 && lnK == 0)
                        {
                            minWeapon[lnJ] = lnI;
                            romData[0x491a1 + (lnJ * 8) + 0] = (byte)(0x80 + lnI);
                        }
                        if (minArmor[lnJ] == 255 && lnK == 1)
                        {
                            minArmor[lnJ] = lnI;
                            romData[0x491a1 + (lnJ * 8) + 1] = (byte)(0x80 + lnI);
                        }
                    }
                }
            }

            for (int lnI = 0; lnI < 8; lnI++)
            {
                if (minWeapon[lnI] != 0x00 && minWeapon[lnI] != 0x01 && minWeapon[lnI] != 0x02)
                {
                    int rEquip = r1.Next() % 100;
                    if (rEquip >= 55)
                    {
                        romData[0x491a1 + (lnI * 8) + 0] = 0x80;
                        romData[0x40c75 + 0x00] += (byte)Math.Pow(2, lnI);
                    }
                    else if (rEquip >= 20)
                    {
                        romData[0x491a1 + (lnI * 8) + 0] = 0x81;
                        romData[0x40c75 + 0x01] += (byte)Math.Pow(2, lnI);
                    }
                    else
                    {
                        romData[0x491a1 + (lnI * 8) + 0] = 0x82;
                        romData[0x40c75 + 0x02] += (byte)Math.Pow(2, lnI);
                    }
                }
                if (minArmor[lnI] != 0x24 && minArmor[lnI] != 0x25 && minArmor[lnI] != 0x26)
                {
                    int rEquip = r1.Next() % 100;
                    if (rEquip >= 55)
                    {
                        romData[0x491a1 + (lnI * 8) + 1] = 0xa4;
                        romData[0x40c75 + 0x24] += (byte)Math.Pow(2, lnI);
                    }
                    else if (rEquip >= 20)
                    {
                        romData[0x491a1 + (lnI * 8) + 1] = 0xa5;
                        romData[0x40c75 + 0x25] += (byte)Math.Pow(2, lnI);
                    }
                    else
                    {
                        romData[0x491a1 + (lnI * 8) + 1] = 0xa6;
                        romData[0x40c75 + 0x26] += (byte)Math.Pow(2, lnI);
                    }
                }
            }
        }

        private void randomizeMonsterZones(Random r1)
        {
            int[] maxMonster = { 0xb4, 0xb4, 0xb4, 0xb4, // 0x00-0x03 - Sea(0x00), wasted (0x01-0x03)
                    11, 21, 22, // 0x04-0x06 - Chapter 1
                    13, 22, 27, 41, 41, 71, // 0x07-0x0c - Chapter 2
                    11, 15, 24, 41, 45, 51, // 0x0d-0x12 - Chapter 4
                    0xbc, // 0x13 - Zenithian Tower
                    10, 15, 24, 31, // 0x14-0x17 - Chapter 3
                    11, 21, 26, 39, 39, 45, // 0x18-0x1d - Chapter 5 -> Symbol Of Faith
                    51, 55, 63, // 0x1e-0x20 - Chapter 5 -> Ship
                    85, 85, 94, 94, // 0x21-0x24 - Chapter 5 -> Padequila Root
                    0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, // 0x25-0x33 - Chapter 5 worldwide
                    0xbc, 0xbc, 0xbd, // 0x34-0x36 - Evil Island/Dark side - need to change this to include nasty bosses
                    0xbc, 0xbc, 0xbc, // 0x37-0x39 - Zenithian Tower - boss needs
                    0xbd, 0xbd, 0xbd, // 0x3a-0x3c - Final Cave - boss needs
                    0xbd, 0xbd, 0xbd, // 0x3d-0x3f - Necrosaro's Palace - boss needs
                    11, 24, 50, 50, // 0x40-0x43 - Chapter 1 - Cave To Izmit, Old Well, Loch Tower(2 zones)
                    33, 69, 69, 33, // 0x44-0x47 - Chapter 4 - Cave west of Kievs (1/4), Aktemto Mine (2/3)
                    12, 12, 39, 39, // 0x48-0x4b - Chapter 3 - Cave north of Lakanaba(2 zones), Cave of Silver Statuette(2 zones)
                    33, 33, 59, 59, // 0x4c-0x4f - Chapter 2 - Cave south of Frenor, Birdsong Tower
                    0xbc, // 0x50 - Chapter 5 - World Tree - boss needs
                    77, 77, // 0x51-0x52 - Chapter 5 - Great Lighthouse
                    104, // 0x53 - Chapter 5 - Cave of the Padequila
                    0xb2, 0xb2, // 0x54-0x55 - Chapter 5 - Cave West Of Kievs
                    0xb2, // Santeem
                    0xb2, 0xb2, // Cascade Cave
                    0xb2, // Shrine of Breaking Waves
                    0xb2, 0xb2, // Cave SE of Gardenbur
                    0xb2, 0xb2, // Royal Crypt
                    0xb2, 0xb2, 0xb2, // Colossus
                    0xbc, 0xbc, // Aktemto Mine
                    0xbc, 0xbc }; // World Tree

            int[] minMonster =
            {
                    0, 0, 0, 0, // 0x00-0x03 - Sea(0x00), wasted (0x01-0x03)
                    0, 0, 0, // 0x04-0x06 - Chapter 1
                    0, 0, 0, 0, 0, 0, // 0x07-0x0c - Chapter 2
                    0, 0, 0, 0, 0, 0, // 0x0d-0x12 - Chapter 4
                    127, // 0x13 - Zenithian Tower
                    0, 0, 0, 0, // 0x14-0x17 - Chapter 3
                    0, 0, 0, 0, 0, 0, // 0x18-0x1d - Chapter 5 -> Symbol Of Faith
                    0, 0, 0, // 0x1e-0x20 - Chapter 5 -> Ship
                    0, 0, 0, 0, // 0x21-0x24 - Chapter 5 -> Padequila Root
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x25-0x33 - Chapter 5 worldwide
                    114, 114, 141, // 0x34-0x36 - Evil Island/Dark side - need to change this to include nasty bosses
                    127, 127, 127, // 0x37-0x39 - Zenithian Tower
                    135, 135, 135, // 0x3a-0x3c - Final Cave - boss needs
                    148, 148, 148, // 0x3d-0x3f - Necrosaro's Palace - boss needs
                    0, 0, 0, 0, // 0x40-0x43 - Chapter 1 - Cave To Izmit, Old Well, Loch Tower(2 zones)
                    0, 0, 0, 0, // 0x44-0x47 - Chapter 4 - Cave west of Kievs (1/4), Aktemto Mine (2/3)
                    0, 0, 0, 0, // 0x48-0x4b - Chapter 3 - Cave north of Lakanaba(2 zones), Cave of Silver Statuette(2 zones)
                    0, 0, 0, 0, // 0x4c-0x4f - Chapter 2 - Cave south of Frenor, Birdsong Tower
                    106, // 0x50 - Chapter 5 - World Tree - boss needs
                    0, 0, // 0x51-0x52 - Chapter 5 - Great Lighthouse
                    0, // 0x53 - Chapter 5 - Cave of the Padequila
                    0, 0, // 0x54-0x55 - Chapter 5 - Cave West Of Kievs
                    0, // Santeem
                    64, 64, // Cascade Cave
                    64, // Shrine of Breaking Waves
                    0, 0, // Cave SE of Gardenbur
                    80, 80, // Royal Crypt
                    94, 94, 94, // Colossus
                    106, 106, // Aktemto Mine
                    106, 106 }; // World Tree

            for (int lnI = 0; lnI <= 0x64; lnI++)
            {
                int byteToUse = 0x612ba + (lnI * 16);
                // Byte 0 - Day/night monsters
                // Byte 1 - Minimum level to guarantee runaway (check this out someday)
                // Byte 2-13 - Actual monsters
                // Byte 14-15 - Special fights (make FF for now)
                for (int lnJ = 0; lnJ < 16; lnJ++)
                {
                    if (lnJ == 0)
                    {
                        romData[byteToUse + lnJ] -= (byte)(romData[byteToUse + lnJ] % 32);
                        romData[byteToUse + lnJ] += 0x1c; // This should ensure that we get to see nearly all of the monsters programmed.
                        continue;
                    }
                    if (lnJ == 1) continue;
                    romData[byteToUse + lnJ] = (byte)(monsterRank[(r1.Next() % (maxMonster[lnI] - minMonster[lnI])) + minMonster[lnI]]);
                    // Redo randomization if Linguar or Imposter are in the proceedings, due to graphical glitches, except the last byte... the solo encounter.
                    if (lnJ != 13 && (romData[byteToUse + lnJ] == 0xba || romData[byteToUse + lnJ] == 0x99)) lnJ--;
                    if (lnJ >= 14) romData[byteToUse + lnJ] = 0xff;
                }
            }

            // Also rearrange boss battles.
            // Mimic, Clay Doll, Chamelion Humanoid, Keeleon I, Balzack I, Saro's Shadow, Clay Doll, Hun, Roric, Vivian, Sampson, Linguar, Tricksy Urchin, Lighthouse Bengal, (14)
            // Keeleon II, Minidemon, Balzack II, Saroknight, Bakor, Rhinoking/Bengal, Esturk, Gigademon, Anderoug(3), Infernus Shadow, Radimvice, Necrosaro, Liclick, (13)
            // Man-eater chest, Rhinoband, Imposter, Leaonar, Necrodain, Minidemon, Bengal (7)
            int[] maxBossLimit = { 0xb4, 0xbd, 27, 0xbd, 69, 50, 0xbd, -1, -1, -1, -1, -1, 33, 104, // 14
                    0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xbc, 0xbc, 0xbc, 0xbc, 0xbc, 0xbc, 0xbc, 60, // 13
                    0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4 }; // 7
            int[] minBossLimit = { 0, 148, 5, 148, 20, 15, 148, -1, -1, -1, -1, -1, 10, 20, // 14
                    0, 106, 0, 0, 0, 106, 106, 148, 148, 148, 148, 148, 20, // 13
                    0, 106, 106, 106, 106, 106, 0 }; // 7
            int[] firstMonster = { 0x62, 0x91, 0x12, 0xbb, 0xb4, 0xb3, 0x91, 0xaf, 0xb0, 0xb1, 0xb2, 0xba, 0xc0, 0xbf,
                    0xbe, -2, 0xb5, 0xc1, 0xc2, 0x9b, 0xbc, 0xb9, 0xb8, 0xb7, 0xb6, 0xae, -2,
                    0x59, -2, -2, -2, -2, -2, -2 };
            for (int lnI = 0; lnI < 34; lnI++)
            {
                if (maxBossLimit[lnI] == -1)
                    continue;
                int byteToUse = 0x6235c + (8 * lnI);

                // 10% chance to keep the battle the way it is now... if there is more than one group involved.
                if (romData[byteToUse + 4 + 1] >= 1 && r1.Next() % 10 == 0)
                    continue;

                for (int lnJ = 0; lnJ < 4; lnJ++)
                    romData[byteToUse + lnJ] = 255;
                for (int lnJ = 4; lnJ < 8; lnJ++)
                    romData[byteToUse + lnJ] = 0;

                // Figure out how many groups of monsters will be involved.
                int groups = (r1.Next() % 4);
                for (int lnJ = 0; lnJ < groups + 1; lnJ++)
                {
                    if (lnJ == 0 && firstMonster[lnI] >= 0)
                        romData[byteToUse + 0] = (byte)firstMonster[lnI];
                    else
                        romData[byteToUse + lnJ] = (byte)(monsterRank[r1.Next() % (maxBossLimit[lnI] - minBossLimit[lnI]) + minBossLimit[lnI]]);
                    romData[byteToUse + lnJ + 4] = (byte)(lnJ == groups ? 8 : 1);
                }
            }
        }

        private void randomizeMonsterResistances(Random r1)
        {
            for (int lnI = 0; lnI < 190; lnI++) // <= 0xc2
            {
                // do not randomize Necrosaro or the metal monsters.
                if (lnI == 0xae || lnI == 0x5c || lnI == 0x75) continue;

                int byteToUse = 0x60056 + (lnI * 22);

                // If silly is selected, adjust enemy resistances +/- 1.
                // If ridiculous is selected, adjust enemy resistances +/- 2.
                // If ludicrous is selected, completely randomize enemy resistances.
                for (int lnJ = 0; lnJ < 5; lnJ++)
                {
                    int res1 = (romData[byteToUse + 15 + lnJ] / 8) % 4;
                    int res2 = romData[byteToUse + 15 + lnJ] / 64;
                    if ((chkC14Random.Checked || chkSoloHero.Checked) && (monsterRank[lnI] == 0xb3 || monsterRank[lnI] == 0x12 || monsterRank[lnI] == 0xaf || monsterRank[lnI] == 0xb0 ||
                        monsterRank[lnI] == 0xb1 || monsterRank[lnI] == 0xb2 || monsterRank[lnI] == 0xba || monsterRank[lnI] == 0xb4))
                    {
                        res1 = res2 = 0;
                    }
                    else if (optMonsterSilly.Checked)
                    {
                        res1 += (r1.Next() % 3) - 1;
                        res2 += (r1.Next() % 3) - 1;
                    }
                    else if (optMonsterMedium.Checked)
                    {
                        if (r1.Next() % 3 != 0)
                        {
                            res1 += (r1.Next() % 5) - 2;
                            res2 += (r1.Next() % 5) - 2;
                        }
                    }
                    else if (optMonsterHeavy.Checked)
                    {
                        res1 = (r1.Next() % 4);
                        res2 = (r1.Next() % 4);
                    }
                    res1 = (res1 < 0 ? 0 : res1 > 3 ? 3 : res1);
                    res2 = (res2 < 0 ? 0 : res2 > 3 ? 3 : res2);

                    romData[byteToUse + 15 + lnJ] = (byte)((romData[byteToUse + 15 + lnJ] % 4) + (res1 * 8) + (res2 * 64));
                }
            }
        }

        private StreamWriter treasureOutput(StreamWriter writer, string name, int[] intArray)
        {
            writer.WriteLine(name);
            string output = "";
            for (int lnI = 0; lnI < intArray.Length; lnI++)
            {
                output += romData[intArray[lnI]].ToString("X2") + " ";
            }
            writer.WriteLine(output);
            writer.WriteLine();
            return writer;
        }

        private void textGet()
        {
            List<string> txtStrings = new List<string>();
            string tempWord = "";
            for (int lnI = 0; lnI < 1913; lnI++)
            {
                int starter = 0x1b2da;
                if (romData[starter + lnI] == 255)
                {
                    txtStrings.Add(tempWord);
                    tempWord = "";
                }
                else if (romData[starter + lnI] >= 0 && romData[starter + lnI] <= 9)
                {
                    tempWord += (char)(romData[starter + lnI] + 39);
                }
                else if (romData[starter + lnI] >= 10 && romData[starter + lnI] <= 35)
                {
                    tempWord += (char)(romData[starter + lnI] + 87);
                }
                else if (romData[starter + lnI] >= 36 && romData[starter + lnI] <= 61)
                {
                    tempWord += (char)(romData[starter + lnI] + 29);
                }
            }
            using (StreamWriter writer = File.CreateText(Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW3Strings.txt")))
            {
                int lnJ = 1;
                foreach (string word in txtStrings)
                {
                    writer.WriteLine(lnJ.ToString("X3") + "-" + word);
                    lnJ++;
                }
            }
        }

        private bool loadRom(bool extra = false)
        {
            try
            {
                romData = File.ReadAllBytes(txtFileName.Text);
                if (extra)
                    romData2 = File.ReadAllBytes(txtCompare.Text);
            }
            catch
            {
                MessageBox.Show("Empty file name(s) or unable to open files.  Please verify the files exist.");
                return false;
            }
            return true;
        }

        private void saveRom()
        {
            string options = "";
            //string options = (chkHalfExpGoldReq.Checked ? "h" : "");
            //options += (chkDoubleXP.Checked ? "d" : "");
            //options += (chkRandStores.Checked ? "1" : "");
            //options += (chkRandStores.Checked ? "2" : "");
            //options += (chkRandStores.Checked ? "3" : "");
            //options += (chkRandStores.Checked ? "4" : "");
            //options += (chkRandStores.Checked ? "5" : "");
            //options += (chkRandStores.Checked ? "6" : "");
            //options += (chkRandStores.Checked ? "7" : "");
            //options += (chkRandStores.Checked ? "8" : "");
            //options += (optNoIntensity.Checked ? "_none" : radSlightIntensity.Checked ? "_slight" : radModerateIntensity.Checked ? "_moderate" : radHeavyIntensity.Checked ? "_heavy" : "_insane");
            string finalFile = Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW4RH_" + txtSeed.Text + "_" + txtFlags.Text + ".nes");
            File.WriteAllBytes(finalFile, romData);
            lblIntensityDesc.Text = "ROM hacking complete!  (" + finalFile + ")";
            txtCompare.Text = finalFile;
        }

        private void forceItemSell()
        {
            int[] forcedItemSell = { 0x16, 0x1c, 0x28, 0x32, 0x34, 0x36, 0x3b, 0x3f, 0x42, 0x48, 0x4b, 0x4c, 0x50, 0x52, 0x53, 0x58, 0x59, 0x69, 0x6f, 0x70, 0x71 };
            for (int lnI = 0; lnI < forcedItemSell.Length; lnI++)
                if (romData[0x11be + forcedItemSell[lnI]] % 32 >= 16) // Not allowed to be sold
                    romData[0x11be + forcedItemSell[lnI]] -= 16; // Now allowed to be sold!

            int[] itemstoAdjust = { 0x16, 0x1c, 0x28, 0x32, 0x34, 0x36, 0x3b, 0x3f, 0x42, 0x48, 0x4b, 0x4c, 0x50, 0x52, 0x53, 0x58, 0x59, 0x5a, 0x69, 0x6f, 0x70, 0x71, // forced items to sell AND...
               0x5f, 0x60, 0x62, 0x64, 0x57, 0x75, 0x55, 0x4e, 0x4f, 0x49 }; // Some other items I want sold (see above)

            int[] itemPriceAdjust = { 5000, 35000, 15000, 10000, 8000, 12000, 10000, 800, 10, 5000, 5000, 8000, 20000, 1000, 1000, 500, 2000, 5000, 5000, 500, 2000, 500,
                5000, 3000, 2500, 5000, 800, 10000, 3000, 2000, 10000, 5000, 1000 };

            for (int lnI = 0; lnI < itemstoAdjust.Length; lnI++)
            {
                // Remove any price adjustment first.
                romData[0x11be + itemstoAdjust[lnI]] -= (byte)(romData[0x11be + itemstoAdjust[lnI]] % 4);
                int priceToUse = (romData[0x123b + itemstoAdjust[lnI]] >= 128 ? romData[0x123b + itemstoAdjust[lnI]] - 128 : romData[0x123b + itemstoAdjust[lnI]]);
                if (itemPriceAdjust[lnI] >= 10000)
                {
                    romData[0x11be + itemstoAdjust[lnI]] += 3; // Now multiply by 1000
                    romData[0x123b + itemstoAdjust[lnI]] = (byte)(romData[0x123b + itemstoAdjust[lnI]] >= 128 ? (itemPriceAdjust[lnI] / 1000) + 128 : itemPriceAdjust[lnI] / 1000);
                }
                else if (itemPriceAdjust[lnI] >= 1000)
                {
                    romData[0x11be + itemstoAdjust[lnI]] += 2; // Now multiply by 100
                    romData[0x123b + itemstoAdjust[lnI]] = (byte)(romData[0x123b + itemstoAdjust[lnI]] >= 128 ? (itemPriceAdjust[lnI] / 100) + 128 : itemPriceAdjust[lnI] / 100);
                }
                else if (itemPriceAdjust[lnI] >= 100)
                {
                    romData[0x11be + itemstoAdjust[lnI]] += 1; // Now multiply by 10
                    romData[0x123b + itemstoAdjust[lnI]] = (byte)(romData[0x123b + itemstoAdjust[lnI]] >= 128 ? (itemPriceAdjust[lnI] / 10) + 128 : itemPriceAdjust[lnI] / 10);
                }
                else
                {
                    romData[0x123b + itemstoAdjust[lnI]] = (byte)(romData[0x123b + itemstoAdjust[lnI]] >= 128 ? itemPriceAdjust[lnI] + 128 : itemPriceAdjust[lnI]);
                }
            }
        }

        private List<int> addTreasure(List<int> currentList, int[] treasureData)
        {
            for (int lnI = 0; lnI < treasureData.Length; lnI++)
                currentList.Add(treasureData[lnI]);
            return currentList;
        }

        private void shuffle(int[] treasureData, Random r1, bool keyItemAvoidance = false)
        {
            // Do not exceed these zones defined for the key items, or you're going to be stuck!
            int[] keyZoneMax = { 13, 13, 23, 40, 45, 53 }; // Cloak of wind, Mirror Of Ra, Golden Key, Jailor's Key, Moon Fragment, Eye Of Malroth
            List<byte> keyItems = new List<byte> { 0x2b, 0x2e, 0x37, 0x39, 0x26, 0x28 }; // When we reach insane randomness, we'll want to know what the key items are so we place them in the appropriate zones...

            // Shuffle each zone 15 times the length of the array for randomness.
            for (int lnI = 0; lnI < 15 * treasureData.Length; lnI++)
            {
                int swap1 = r1.Next() % treasureData.Length;
                int swap2 = r1.Next() % treasureData.Length;

                // Don't shuffle if key items would be swapped into inaccessible areas.
                if (keyItemAvoidance)
                {
                    int position1 = keyItems.IndexOf(romData[treasureData[swap1]]);
                    int position2 = keyItems.IndexOf(romData[treasureData[swap2]]);
                    if (position1 > -1 && swap2 > keyZoneMax[position1])
                        continue;
                    if (position2 > -1 && swap1 > keyZoneMax[position2])
                        continue;
                }

                swap(treasureData[swap1], treasureData[swap2]);
            }
        }

        private void swap(int firstAddress, int secondAddress)
        {
            byte holdAddress = romData[secondAddress];
            romData[secondAddress] = romData[firstAddress];
            romData[firstAddress] = holdAddress;
        }

        private int[] swapArray(int[] array, int first, int second)
        {
            int holdAddress = array[second];
            array[second] = array[first];
            array[first] = holdAddress;
            return array;
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (!loadRom(true)) return;
            using (StreamWriter writer = File.CreateText(Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW3Compare.txt")))
            {
                //for (int lnI = 0; lnI < 0x8a; lnI++)
                //    compareComposeString("monsters" + lnI.ToString("X2"), writer, (0x32e3 + (23 * lnI)), 23);

                //compareComposeString("treasure-Burland-1", writer, 0x7bd1d, 1);
                //compareComposeString("treasure-IzmitCave", writer, 0x7bf37, 2);
                //compareComposeString("treasure-Izmit", writer, 0x7bd6a, 1);
                //compareComposeString("treasure-OldWell", writer, 0x7bf15, 3);
                //compareComposeString("treasure-OldWellp2", writer, 0x7bdb7, 1);
                //compareComposeString("treasure-OldWellp3", writer, 0x7b935, 1);
                //compareComposeString("treasure-Loch", writer, 0x7bf47, 6);

                //compareComposeString("treasure-Santeemp1", writer, 0x7bd0f, 1);
                //compareComposeString("treasure-Santeemp2", writer, 0x7bd16, 1);
                //compareComposeString("treasure-Santeemp3", writer, 0x7bd08, 1);
                //compareComposeString("treasure-Santeem-5", writer, 0x7beca, 3);
                //compareComposeString("treasure-Tempe", writer, 0x7bdc7, 1);
                //compareComposeString("treasure-FrenorCave", writer, 0x7bf10, 5);
                //compareComposeString("treasure-Bazaarp1", writer, 0x7bd4e, 1);
                //compareComposeString("treasure-Bazaarp2", writer, 0x7bd55, 1);
                //compareComposeString("treasure-Birdsongp1", writer, 0x7bf41, 3);
                //compareComposeString("treasure-Birdsongp2", writer, 0x7b8f3, 1);
                //compareComposeString("treasure-Endorp1", writer, 0x7beda, 5);
                //compareComposeString("treasure-Endorp2", writer, 0x7bd2b, 1);

                compareComposeString("treasure-Burland-5", writer, 0x7bd1d, 1);

            }
            lblIntensityDesc.Text = "Comparison complete!  (DW3Compare.txt)";
        }

        private StreamWriter compareComposeString(string intro, StreamWriter writer, int startAddress, int length, int skip = 1, string delimiter = "")
        {
            if (delimiter == "")
            {
                writer.WriteLine(intro);
                string final = "";
                string final2 = "";
                for (int lnI = 0; lnI < length; lnI += skip)
                {
                    final += romData[startAddress + lnI].ToString("X2") + " ";
                    if (lnI % 16 == 15)
                    {
                        writer.WriteLine(final);
                        final = "";
                    }
                }
                writer.WriteLine(final);
                if (length >= 16) writer.WriteLine();
                for (int lnI = 0; lnI < length; lnI += skip)
                {
                    final2 += romData2[startAddress + lnI].ToString("X2") + " ";
                    if (lnI % 16 == 15)
                    {
                        writer.WriteLine(final2);
                        final2 = "";
                    }
                }
                writer.WriteLine(final2);
                writer.WriteLine();
            }
            else
            {
                writer.WriteLine(intro);

                string final = "";
                for (int lnI = 0; lnI < length; lnI += skip)
                {
                    final += romData[startAddress + lnI].ToString("X2") + " ";
                    if (delimiter == "g128" && romData[startAddress + lnI] >= 128)
                    {
                        writer.WriteLine(final);
                        final = "";
                    }
                }
                writer.WriteLine(final);
                writer.WriteLine("---------------- VS -------------");
                final = "";
                for (int lnI = 0; lnI < length; lnI += skip)
                {
                    final += romData2[startAddress + lnI].ToString("X2") + " ";
                    if (delimiter == "g128" && romData2[startAddress + lnI] >= 128)
                    {
                        writer.WriteLine(final);
                        final = "";
                    }
                }
                writer.WriteLine(final);
                writer.WriteLine("");
            }
            return writer;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (txtFileName.Text != "")
                using (StreamWriter writer = File.CreateText("lastFile4.txt"))
                {
                    writer.WriteLine(txtFileName.Text);
                    writer.WriteLine(txtCompare.Text);
                    writer.WriteLine(txtC1Name1.Text);
                    writer.WriteLine(txtC1Name2.Text);
                    writer.WriteLine(txtC2Name1.Text);
                    writer.WriteLine(txtC2Name2.Text);
                    writer.WriteLine(txtC2Name3.Text);
                    writer.WriteLine(txtC3Name1.Text);
                    writer.WriteLine(txtC3Name2.Text);
                    writer.WriteLine(txtC3Name3.Text);
                    writer.WriteLine(txtC4Name1.Text);
                    writer.WriteLine(txtC4Name2.Text);
                    writer.WriteLine(txtC4Name3.Text);
                    writer.WriteLine(txtC5Name1.Text);
                    writer.WriteLine(txtC5Name2.Text);
                    writer.WriteLine(txtC5Name3.Text);
                    writer.WriteLine(txtC5Name4.Text);

                    writer.WriteLine(txtSeed.Text);
                    writer.WriteLine(txtFlags.Text);
                }
        }

        private void txtFileName_Leave(object sender, EventArgs e)
        {
            runChecksum();
        }

        private void btnCompareBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtCompare.Text = openFileDialog1.FileName;
            }
        }

        private void textOutput()
        {
            loadRom(false);
            using (StreamWriter writer = File.CreateText(Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW3TextOutput.txt")))
            {
                for (int lnI = 0; lnI < 96; lnI++)
                    outputComposeString("monstersZones" + (lnI).ToString("X2"), writer, (0xaeb + (15 * lnI)), 15);

                for (int lnI = 0; lnI < 19; lnI++)
                    outputComposeString("monstersZoneSpecial" + (lnI + 1).ToString("X2"), writer, (0x108b + (6 * lnI)), 6);

                for (int lnI = 0; lnI < 140; lnI++)
                    outputComposeString("monsters" + (lnI).ToString("X2"), writer, (0x32e3 + (23 * lnI)), 23);

                for (int lnI = 0; lnI < 21; lnI++)
                    outputComposeString("bosses" + (lnI).ToString("X2"), writer, (0x8ee + (2 * lnI)), 2, 1, 43);
            }
            lblIntensityDesc.Text = "Text output complete!  (DW3TextOutput.txt)";
        }

        private StreamWriter outputComposeString(string intro, StreamWriter writer, int startAddress, int length, int skip = 1, int duplicate = 0)
        {
            string final = "";
            for (int lnI = 0; lnI < length; lnI += skip)
            {
                final += romData[startAddress + lnI].ToString("X2") + " ";
            }
            if (duplicate != 0)
            {
                for (int lnI = duplicate; lnI < length + duplicate; lnI += skip)
                {
                    final += romData[startAddress + lnI].ToString("X2") + " ";
                }
            }
            writer.WriteLine(intro);
            writer.WriteLine(final);
            writer.WriteLine();
            return writer;
        }

        private void btnMonsterOutput_Click(object sender, EventArgs e)
        {
            loadRom(false);
            using (StreamWriter writer = File.CreateText(Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW4MonsterOutput.txt")))
            {
                for (int lnI = 0; lnI < 214; lnI++)
                    outputComposeString("monsters" + (lnI).ToString("X2"), writer, (0x60054 + (22 * lnI)), 22);

                for (int lnI = 0; lnI < 101; lnI++)
                    outputComposeString("monstersZones" + (lnI).ToString("X2"), writer, (0x612ba + (16 * lnI)), 16);

                for (int lnI = 0; lnI < 34; lnI++)
                    outputComposeString("bossFights" + (lnI).ToString("X2"), writer, (0x6235c + (8 * lnI)), 8);
            }
            lblIntensityDesc.Text = "Text output complete!  (DW4MonsterOutput.txt)";
        }

        private void chkSoloHero_CheckedChanged(object sender, EventArgs e)
        {
            cboSoloHero.Enabled = chkSoloHero.Checked;
            chkSoloCanEquipAll.Enabled = chkSoloHero.Checked;
            chkC14Random.Enabled = !chkSoloHero.Checked;
            determineFlags(null, null);
        }

        private void chkShop1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShop1.Checked) chkShop25K.Checked = false;
            determineFlags(null, null);
        }

        private void chkShop25K_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShop25K.Checked) chkShop1.Checked = false;
            determineFlags(null, null);
        }

        private void cmdStatOutput_Click(object sender, EventArgs e)
        {
            loadRom(false);
            using (StreamWriter writer = File.CreateText(Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW4StatOutput.txt")))
            {
                for (int lnI = 0; lnI < 6; lnI++)
                    for (int lnJ = 0; lnJ < 8; lnJ++)
                        outputStatString("stats" + lnI.ToString() + "-" + lnJ.ToString(), writer, (0x4a15b + (48 * lnI) + (6 * lnJ)), 6);
            }
            lblIntensityDesc.Text = "Text output complete!  (DW4MonsterOutput.txt)";
        }

        private StreamWriter outputStatString(string intro, StreamWriter writer, int startAddress, int length, int skip = 1, int duplicate = 0)
        {
            int level = 1;
            int multiplier = romData[startAddress] % 128;
            multiplier = (multiplier >= 96 ? 3 : multiplier >= 64 ? 2 : multiplier >= 32 ? 1 : 0);
            string final = "";
            for (int lnI = 0; lnI < 5; lnI += skip)
            {
                if (romData[startAddress + lnI] == 99) level = 99;
                level += (romData[startAddress + lnI] % 32);
                if (level > 99) level = 99;
                final += level.ToString() + "-";
            }
            final += "B" + romData[startAddress + 5] + "-M" + multiplier;

            writer.WriteLine(intro);
            writer.WriteLine(final);
            writer.WriteLine();
            return writer;
        }

        public void determineFlags(object sender, EventArgs e)
        {
            if (loading) return;

            string flags = "";

            if (chkSoloHero.Checked)
            {
                flags += "O";
                flags += ((string)cboSoloHero.SelectedItem == "Hero" ? 0 :
                          (string)cboSoloHero.SelectedItem == "Cristo" ? 1 :
                          (string)cboSoloHero.SelectedItem == "Nara" ? 2 :
                          (string)cboSoloHero.SelectedItem == "Mara" ? 3 :
                          (string)cboSoloHero.SelectedItem == "Brey" ? 4 :
                          (string)cboSoloHero.SelectedItem == "Taloon" ? 5 :
                          (string)cboSoloHero.SelectedItem == "Ragnar" ? 6 : 7);
            }
            flags += (chkSoloCanEquipAll.Checked ? "o" : "");
            flags += (chkC14Random.Checked ? "H" : "");
            if ((string)cboXPAdjustment.SelectedItem != "100%" && (string)cboXPAdjustment.SelectedItem != "")
            {
                flags += "X";
                flags += ((string)cboXPAdjustment.SelectedItem == "50%" ? 0 :
                    (string)cboXPAdjustment.SelectedItem == "100%" ? 1 :
                    (string)cboXPAdjustment.SelectedItem == "150%" ? 2 :
                    (string)cboXPAdjustment.SelectedItem == "200%" ? 3 :
                    (string)cboXPAdjustment.SelectedItem == "300%" ? 4 :
                    (string)cboXPAdjustment.SelectedItem == "400%" ? 5 : 6);
            }
            flags += (chkXPRandom.Checked ? "x" : "");
            if ((string)cboGoldAdjustment.SelectedItem != "100%" && (string)cboGoldAdjustment.SelectedItem != "")
            {
                flags += "G";
                flags += ((string)cboGoldAdjustment.SelectedItem == "50%" ? 0 :
                    (string)cboGoldAdjustment.SelectedItem == "100%" ? 1 :
                    (string)cboGoldAdjustment.SelectedItem == "150%" ? 2 : 3);
            }
            flags += (chkGoldRandom.Checked ? "g" : "");
            if ((string)cboEncounterRate.SelectedItem != "x1" && (string)cboEncounterRate.SelectedItem != "")
            {
                flags += "E";
                flags += ((string)cboEncounterRate.SelectedItem == "1/4" ? 0 :
                    (string)cboEncounterRate.SelectedItem == "1/2" ? 1 :
                    (string)cboEncounterRate.SelectedItem == "3/4" ? 2 :
                    (string)cboEncounterRate.SelectedItem == "x1" ? 3 :
                    (string)cboEncounterRate.SelectedItem == "x1.5" ? 4 :
                    (string)cboEncounterRate.SelectedItem == "x2" ? 5 :
                    (string)cboEncounterRate.SelectedItem == "x3" ? 6 : 7);
            }
            flags += (chkRandomMonsterZones.Checked ? "Z" : "");
            flags += (chkSpeedUpBattles.Checked ? "B" : "");
            flags += (chkRandomHeroEquip.Checked ? "E" : "");
            flags += (chkRandomMonsterStats.Checked ? "s" : "");
            flags += (chkRandomTreasures.Checked ? "T" : "");
            flags += (chkRandomMonsterResistances.Checked ? "R" : "");
            flags += (chkRandomStores.Checked ? "M" : "");
            flags += (chkRandomHeroStats.Checked ? "S" : "");
            flags += (chkC5ControlAllChars.Checked ? "C" : "");
            flags += (chkRandomMonsterAttacks.Checked ? "A" : "");

            flags += (optMonsterLight.Checked ? "_r1" : "");
            flags += (optMonsterSilly.Checked ? "_r2" : "");
            flags += (optMonsterMedium.Checked ? "_r3" : "");
            flags += (optMonsterHeavy.Checked ? "_r4" : "");
            if (chkCh2AwardXPTournament.Checked)
            {
                flags += "_2";
                flags += (chkCh2AwardXPTournament.Checked ? "T" : "");
            }
            if (chkShop1.Checked || chkShop25K.Checked || chkTunnel1.Checked)
            {
                flags += "_3";
                flags += (chkShop1.Checked ? "s" : "");
                flags += (chkShop25K.Checked ? "S" : "");
                flags += (chkTunnel1.Checked ? "T" : "");
            }

            txtFlags.Text = flags;
        }

        private void determineChecks(object sender, EventArgs e)
        {
            string[] flags = txtFlags.Text.Split('_');

            foreach (string flag in flags)
            {
                if (flag.Substring(0, 1).Contains("1"))
                {
                    // nothing implemented yet
                } else if (flag.Substring(0, 1).Contains("2"))
                {
                    chkCh2AwardXPTournament.Checked = flag.Contains("T");
                }
                else if (flag.Substring(0, 1).Contains("3"))
                {
                    chkShop1.Checked = flag.Contains("s");
                    chkShop25K.Checked = flag.Contains("S");
                    chkTunnel1.Checked = flag.Contains("T");
                }
                else if (flag.Substring(0, 1).Contains("4"))
                {
                    // nothing implemented yet
                }
                else if (flag.Substring(0, 1).Contains("r"))
                {
                    optMonsterLight.Checked = flag.Contains("r1");
                    optMonsterSilly.Checked = flag.Contains("r2");
                    optMonsterMedium.Checked = flag.Contains("r3");
                    optMonsterHeavy.Checked = flag.Contains("r4");
                }
                else
                {
                    // everything else!
                    chkSoloHero.Checked = flag.Contains("O");
                    cboSoloHero.SelectedItem = (flag.Contains("O0") ? "Hero" : flag.Contains("O0") ? "Cristo" : flag.Contains("O0") ? "Nara" : 
                        flag.Contains("O0") ? "Mara" : flag.Contains("O0") ? "Brey" : flag.Contains("O0") ? "Taloon" : flag.Contains("O0") ? "Ragnar" : "Alena");
                    chkSoloCanEquipAll.Checked = flag.Contains("o");
                    chkC14Random.Checked = flag.Contains("H");
                    cboXPAdjustment.SelectedItem = (flag.Contains("X0") ? "50%" : flag.Contains("X2") ? "150%" : flag.Contains("X3") ? "200%" :
                        flag.Contains("X4") ? "300%" : flag.Contains("X5") ? "400%" : flag.Contains("X6") ? "500%" : "100%");
                    chkXPRandom.Checked = flag.Contains("x");
                    cboGoldAdjustment.SelectedItem = (flag.Contains("G0") ? "50%" : flag.Contains("G2") ? "150%" : flag.Contains("G3") ? "200%" : "100%");
                    chkGoldRandom.Checked = flag.Contains("g");
                    cboEncounterRate.SelectedItem = (flag.Contains("E0") ? "1/4" : flag.Contains("E1") ? "1/2" : flag.Contains("E2") ? "3/4" :
                        flag.Contains("E4") ? "x1.5" : flag.Contains("E5") ? "x2" : flag.Contains("E6") ? "x3" : flag.Contains("E7") ? "x4" : "x1");
                    chkRandomMonsterZones.Checked = flag.Contains("Z");
                    chkSpeedUpBattles.Checked = flag.Contains("B");
                    chkRandomHeroEquip.Checked = flag.Contains("E");
                    chkRandomMonsterStats.Checked = flag.Contains("s");
                    chkRandomTreasures.Checked = flag.Contains("T");
                    chkRandomMonsterResistances.Checked = flag.Contains("R");
                    chkRandomStores.Checked = flag.Contains("M");
                    chkRandomHeroStats.Checked = flag.Contains("S");
                    chkC5ControlAllChars.Checked = flag.Contains("C");
                    chkRandomMonsterAttacks.Checked = flag.Contains("A");
                }
            }
        }
    }
}
