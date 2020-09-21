using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.Design;
using System.Security.Policy;

namespace DW4RandoHacker
{
	public partial class DW4Rando : Form
	{
		bool loading = true;
		byte[] romData;
		byte[] romData2;

		int[] monsterRank = // after 0x55, 0x??????, - bisonhawk unknown
            {
					0x01, 0x00, 0x03, 0x02, 0x05, 0x08, 0x07, 0x09, 0x06, 0x0b, 0x0e, 0x0a, 0x11, 0x0d, 0x0f, 0x14, // 6 XP - 0-15
                    0x0c, 0x1c, 0x1a, 0x18, 0x13, 0x10, 0x1f, 0x26, 0x16, 0x1e, 0x19, 0x17, 0x24, 0x1b, 0x22, 0x23, // 15 XP - 16-31
                    0x15, 0x1d, 0x5c, 0x2a, 0x20, 0x27, 0x25, 0x21, 0x43, 0x28, 0x2f, 0x04, 0x31, 0x2c, 0x3c, 0x29, // 27 XP - 32-47
                    0x3d, 0x2d, 0x36, 0x45, 0x2e, 0x38, 0x39, 0x33, 0x42, 0x3e, 0x58, 0x4d, 0x40, 0x4a, 0x32, 0x47, // 45 XP - 48-63
                    0x2b, 0x35, 0x52, 0x48, 0x46, 0x37, 0x4c, 0xaf, 0x34, 0x5e, 0x3a, 0x4f, 0x66, 0xb3, 0x3b, 0x49, // 77 XP - 64-79
                    0xb0, 0xb1, 0x56, 0x41, 0x51, 0x50, 0x55, 0x57, 0x44, 0x5a, 0x3f, 0xb2, 0xba, 0x30, 0x53, 0x60, // 104 XP - 80-95
                    0x5b, 0x75, 0x5f, 0x4b, 0x68, 0x63, 0x5d, 0x54, 0x6b, 0x61, 0x67, 0x6e, 0x6a, 0x76, 0x12, 0x69, // 96-111
					0x70, 0x59, 0x78, 0x72, 0xab, 0x7c, 0x7d, 0x73, 0x71, 0x6c, 0x6f, 0x7f, 0x77, 0x74, 0x64, 0x86, // 112-127
					0x7a, 0x6d, 0x79, 0x7b, 0x80, 0x81, 0x7e, 0x87, 0x83, 0x88, 0x85, 0x82, 0x84, 0x62, 0x8c, 0x97, // 128-143
					0x89, 0x8a, 0x8d, 0x8b, 0x93, 0x90, 0xc0, 0x9c, 0x8e, 0x98, 0x95, 0xb4, 0x96, 0x9f, 0x9a, 0x99, // 144-159
					0x92, 0x9d, 0xa2, 0x94, 0x9e, 0x91, 0x8f, 0xa1, 0xa9, 0x9b, 0xa0, 0xa6, 0xaa, 0xac, 0x65, 0xa8, // 160-175
					0xb8, 0xa3, 0xa4, 0xa5, 0xa7, 0xbf, 0xb9, 0xbe, 0xb7, 0xb6, 0xb5, 0xc1, 0xc2, 0xbc // 15000 - bosses start at 0xb5 - 176-189
            };
		int[] firstMonster = { 0x62, 0x91, 0x12, 0xbb, 0xb4, 0xb3, 0x91, 0xaf, 0xb0, 0xb1, 0xb2, 0xba, 0xc0, 0xbf,
					0xbe, -2, 0xb5, 0xc1, 0xc2, 0x9b, 0xbc, 0xb9, 0xb8, 0xb7, 0xb6, 0xae, -2,
					0x59, -2, -2, -2, -2, -2, -2 };

		int[,] map = new int[256, 256];
		int[,] map2 = new int[139, 158];
		int[,] island = new int[256, 256];
		int[,] island2 = new int[139, 158];
		int[,] zone = new int[16, 16];
		int[] maxIsland = new int[11];
		List<int> islands = new List<int>();

		List<byte> L1Weapons = new List<byte> { 0x00, 0x01, 0x02, 0x04, 0x09, 0x0b };
		List<byte> L2Weapons = new List<byte> { 0x03, 0x04, 0x05, 0x06, 0x08, 0x0a, 0x0b, 0x0c, 0x0d, 0x19, 0x23 };
		List<byte> L3Weapons = new List<byte> { 0x06, 0x07, 0x0c, 0x0f, 0x10, 0x11, 0x15, 0x16, 0x17, 0x18, 0x22, 0x23 };
		List<byte> L4Weapons = new List<byte> { 0x0e, 0x1a, 0x1b, 0x1c, 0x1d, 0x1f, 0x20 };
		List<byte> L12Weapons;
		List<byte> L34Weapons;
		List<byte> AllWeapons;

		List<byte> L1Armor = new List<byte> { 0x24, 0x25, 0x26, 0x27, 0x2b, 0x2c, 0x30, 0x3d, 0x46 };
		List<byte> L2Armor = new List<byte> { 0x27, 0x28, 0x29, 0x2a, 0x2d, 0x2f, 0x33, 0x3e, 0x3f, 0x47, 0x4a, 0x4d };
		List<byte> L3Armor = new List<byte> { 0x2a, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x38, 0x39, 0x40, 0x43, 0x48, 0x49, 0x4d };
		List<byte> L4Armor = new List<byte> { 0x2e, 0x3b, 0x41, 0x42, 0x45, 0x4e, 0x4f };
		List<byte> L12Armor;
		List<byte> L34Armor;
		List<byte> AllArmor;

		List<byte> L1Items = new List<byte> { 0x53, 0x54, 0x55, 0x56, 0x58 };
		List<byte> L2Items = new List<byte> { 0x53, 0x54, 0x55, 0x56, 0x58, 0x5e, 0x74 };
		List<byte> L3Items = new List<byte> { 0x53, 0x54, 0x55, 0x56, 0x58, 0x5a, 0x5e, 0x74 };
		List<byte> L4Items = new List<byte> { 0x59, 0x5a, 0x5b, 0x61, 0x62, 0x63, 0x64, 0x65, 0x59, 0x5a, 0x5b, 0x61, 0x62, 0x63, 0x64, 0x65, 0x50 };
		List<byte> L12Items;
		List<byte> L34Items;
		List<byte> AllItems;

		List<byte> L1All;
		List<byte> L2All;
		List<byte> L12All;
		List<byte> L3All;
		List<byte> L4All;
		List<byte> L34All;
		List<byte> L1234All;

		bool oSwapBossStats;
		bool oSwapMonsterStats;
		bool oBasePriceOnPower;
		bool oStoreNoSeeds;
		bool oSmallMap;
		bool oRandomizeMap;
		bool oCh1Moat;
		bool oCh1FlyingShoes;
		bool oCh1InstantWell;
		bool oCh2EndorEntry;
		bool oCh2InstantWallKick;
		bool oCh2AwardXPTournament;
		bool oCh3BuildTunnel;
		bool oCh3BuildBridges;
		bool oCh3Tunnel1;
		bool oCh3Shop25K;
		bool oCh3Shop1;
		bool oCh4GunpowderJar;
		bool oCh4BoardingPass;
		bool oCh5GasCanister;
		bool oInstantFinalCave;
		bool oCh5SymbolOfFaith;
		bool oCh5NoZGear;
		bool oCh5AneauxNecro;
		bool oCh5BlowUpHometown;
		bool oCh5ControlAllChars;
		bool oRandomizeNPCs;
		bool oC14Random;
		bool oSoloHero;
		bool oChapter5Start;
		bool oSpeedUpMusic;
		bool oSpeedyText;
		bool oDoubleWalking;
		bool oSpeedUpBattles;

		int oGoldRandom;
		int oXPRandom;
		int oEncounterRate;
		int oGoldBoost;
		int oXPBoost;
		int oMonsterResistances;
		int oMonsterZones;
		int oMonsterAttacks;
		int oMonsterStats;
		int oNecroDifficulty;
		int oEquipChances;
		int oTreasures;
		int oHeroSpells;
		int oHeroStats;
		int oStoreAvailability;
		int oNPCs;
		int oTaloonInsanityChance;
		int oTaloonInsanity;
		int oCriticalHitChance;
		int oSoloHeroSelection;
		int oEquipPowers;
		int oStorePrices;
		int oTaloonInsanityMoves;
		int oLevelCrit;
		int oMonsterDropChance;
		int oMonsterDrops;

		public DW4Rando()
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
			// Item list initialization
			L12Weapons = L1Weapons.Concat(L2Weapons).Distinct().ToList();
			L34Weapons = L3Weapons.Concat(L4Weapons).Distinct().ToList();
			AllWeapons = L12Weapons.Concat(L34Weapons).Distinct().ToList();

			L12Armor = L1Armor.Concat(L2Armor).Distinct().ToList();
			L34Armor = L3Armor.Concat(L4Armor).Distinct().ToList();
			AllArmor = L12Armor.Concat(L34Armor).Distinct().ToList();

			L12Items = L1Items.Concat(L2Items).Distinct().ToList();
			L34Items = L3Items.Concat(L4Items).Distinct().ToList();
			AllItems = L12Items.Concat(L34Items).Distinct().ToList();

			L1All = L1Weapons.Concat(L1Armor).Concat(L1Items).Distinct().ToList();
			L2All = L2Weapons.Concat(L2Armor).Concat(L2Items).Distinct().ToList();
			L12All = L1All.Concat(L2All).Distinct().ToList();
			L3All = L3Weapons.Concat(L3Armor).Concat(L3Items).Distinct().ToList();
			L4All = L4Weapons.Concat(L4Armor).Concat(L4Items).Distinct().ToList();
			L34All = L3All.Concat(L4All).Distinct().ToList();
			L1234All = L12All.Concat(L34All).Distinct().ToList();

			cboXPBoost.SelectedIndex = 1;
			cboGoldBoost.SelectedIndex = 1;

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
				txtFlags.Text = "000000000008e0022300";
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
				loading = false;
				determineChecks(null, null);
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
			saveRom(false);
		}

		private bool hackRom()
		{
			Random r1;
			try
			{
				if (txtSeed.Text == "whoa") r1 = new Random((int)DateTime.Now.Ticks % 2147483647);
				else r1 = new Random(int.Parse(txtSeed.Text));
			}
			catch
			{
				MessageBox.Show("Invalid seed.  It must be a number from 0 to 2147483647.");
				return false;
			}

			if (chkRandomFlags.Checked)
			{
				bool flagLegal = false;

				while (!flagLegal)
				{
					oSwapBossStats = (r1.Next() % 2 == 1);
					oSwapMonsterStats = (r1.Next() % 2 == 1);
					oBasePriceOnPower = (r1.Next() % 2 == 1);
					oStoreNoSeeds = (r1.Next() % 2 == 1);
					oSmallMap = (r1.Next() % 2 == 1);
					oRandomizeMap = (r1.Next() % 2 == 1);
					oCh1Moat = (r1.Next() % 2 == 1);
					oCh1FlyingShoes = (r1.Next() % 2 == 1);
					oCh1InstantWell = (r1.Next() % 2 == 1);
					oCh2EndorEntry = (r1.Next() % 2 == 1);
					oCh2InstantWallKick = (r1.Next() % 2 == 1);
					oCh2AwardXPTournament = (r1.Next() % 2 == 1);
					oCh3BuildTunnel = (r1.Next() % 2 == 1);
					oCh3BuildBridges = (r1.Next() % 2 == 1);
					oCh3Tunnel1 = (r1.Next() % 2 == 1);
					oCh3Shop25K = (r1.Next() % 2 == 1);
					oCh3Shop1 = (r1.Next() % 2 == 1);
					oCh4GunpowderJar = (r1.Next() % 2 == 1);
					oCh4BoardingPass = (r1.Next() % 2 == 1);
					oCh5GasCanister = (r1.Next() % 2 == 1);
					oInstantFinalCave = (r1.Next() % 2 == 1);
					oCh5SymbolOfFaith = (r1.Next() % 2 == 1);
					oCh5NoZGear = (r1.Next() % 2 == 1);
					oCh5AneauxNecro = (r1.Next() % 2 == 1);
					oCh5BlowUpHometown = (r1.Next() % 2 == 1);
					oCh5ControlAllChars = (r1.Next() % 2 == 1);
					oRandomizeNPCs = (r1.Next() % 2 == 1);
					oC14Random = (r1.Next() % 2 == 1);
					oSoloHero = (r1.Next() % 2 == 1);
					oChapter5Start = (r1.Next() % 2 == 1);

					oGoldRandom = (r1.Next() % 6);
					oXPRandom = (r1.Next() % 6);
					oEncounterRate = (r1.Next() % 8);
					oGoldBoost = (r1.Next() % 8);
					oXPBoost = (r1.Next() % 8);
					oMonsterResistances = (r1.Next() % 4);
					oMonsterZones = (r1.Next() % 5);
					oMonsterAttacks = (r1.Next() % 5);
					oMonsterStats = (r1.Next() % 6);
					oNecroDifficulty = (r1.Next() % cboNecroDifficulty.Items.Count);
					oEquipChances = (r1.Next() % 8);
					oTreasures = (r1.Next() % 8);
					oHeroSpells = (r1.Next() % 5);
					oHeroStats = (r1.Next() % 5);
					oStoreAvailability = (r1.Next() % 8);
					oNPCs = (r1.Next() % 7);
					oTaloonInsanityChance = (r1.Next() % 8);
					oTaloonInsanity = (r1.Next() % 3);
					oCriticalHitChance = (r1.Next() % 8);
					oSoloHeroSelection = (r1.Next() % 8);
					oEquipPowers = (r1.Next() % 6);
					oStorePrices = (r1.Next() % 7);
					oTaloonInsanityMoves = (r1.Next() % 8);
					oLevelCrit = (r1.Next() % 4);
					oMonsterDropChance = (r1.Next() % 5);
					oMonsterDrops = (r1.Next() % 6);

					flagLegal = true;
					// We really need to be concerned about low XP seeds; we have to require seeds in stores.
					if (oXPBoost == 1 && oStoreNoSeeds)
						flagLegal = false;
					// If we have no XP at all, monsters will have to drop seeds frequently as well.
					if (oXPBoost == 0 && (oStoreNoSeeds || oMonsterDropChance == 0 || oMonsterDropChance == 2 || oMonsterDrops != 3))
						flagLegal = false;
					// If all monsters drop zero gold, then we have to have stores sell stuff for free or monsters need to drop stuff frequently.
					if (oGoldBoost == 0 && (oStorePrices != 6 || (oMonsterDropChance != 1 && oMonsterDropChance != 4)))
						flagLegal = false;
					// If we have both no gold or XP, then stores will have to be free, contain seeds, and monsters will have to drop 100% of the time.
					if (oGoldBoost == 0 && oXPBoost == 0 && oStoreNoSeeds && oMonsterDropChance != 4)
						flagLegal = false;
				}
			}
			else
			{
				oSwapBossStats = chkSwapBossStats.Checked;
				oSwapMonsterStats = chkSwapMonsterStats.Checked;
				oBasePriceOnPower = chkBasePriceOnPower.Checked;
				oStoreNoSeeds = chkStoreNoSeeds.Checked;
				oSmallMap = chkSmallMap.Checked;
				oRandomizeMap = chkRandomizeMap.Checked;
				oCh1Moat = chkCh1Moat.Checked;
				oCh1FlyingShoes = chkCh1FlyingShoes.Checked;
				oCh1InstantWell = chkCh1InstantWell.Checked;
				oCh2EndorEntry = chkCh2EndorEntry.Checked;
				oCh2InstantWallKick = chkCh2InstantWallKick.Checked;
				oCh2AwardXPTournament = chkCh2AwardXPTournament.Checked;
				oCh3BuildTunnel = chkCh3BuildTunnel.Checked;
				oCh3BuildBridges = chkCh3BuildBridges.Checked;
				oCh3Tunnel1 = chkCh3Tunnel1.Checked;
				oCh3Shop25K = chkCh3Shop25K.Checked;
				oCh3Shop1 = chkCh3Shop1.Checked;
				oCh4GunpowderJar = chkCh4GunpowderJar.Checked;
				oCh4BoardingPass = chkCh4BoardingPass.Checked;
				oCh5GasCanister = chkCh5GasCanister.Checked;
				oInstantFinalCave = chkInstantFinalCave.Checked;
				oCh5SymbolOfFaith = chkCh5SymbolOfFaith.Checked;
				oCh5NoZGear = chkCh5NoZGear.Checked;
				oCh5AneauxNecro = chkCh5AneauxNecro.Checked;
				oCh5BlowUpHometown = chkCh5BlowUpHometown.Checked;
				oCh5ControlAllChars = chkCh5ControlAllChars.Checked;
				oRandomizeNPCs = chkRandomizeNPCs.Checked;
				oC14Random = chkC14Random.Checked;
				oSoloHero = chkSoloHero.Checked;
				oChapter5Start = chkChapter5Start.Checked;

				oGoldRandom = cboGoldRandom.SelectedIndex;
				oXPRandom = cboXPRandom.SelectedIndex;
				oEncounterRate = cboEncounterRate.SelectedIndex;
				oGoldBoost = cboGoldBoost.SelectedIndex;
				oXPBoost = cboXPBoost.SelectedIndex;
				oMonsterResistances = cboMonsterResistances.SelectedIndex;
				oMonsterZones = cboMonsterZones.SelectedIndex;
				oMonsterAttacks = cboMonsterAttacks.SelectedIndex;
				oMonsterStats = cboMonsterStats.SelectedIndex;
				oNecroDifficulty = cboNecroDifficulty.SelectedIndex;
				oEquipChances = cboEquipChances.SelectedIndex;
				oTreasures = cboTreasures.SelectedIndex;
				oHeroSpells = cboHeroSpells.SelectedIndex;
				oHeroStats = cboHeroStats.SelectedIndex;
				oStoreAvailability = cboStoreAvailability.SelectedIndex;
				oNPCs = cboNPCs.SelectedIndex;
				oTaloonInsanityChance = cboTaloonInsanityChance.SelectedIndex;
				oTaloonInsanity = cboTaloonInsanity.SelectedIndex;
				oCriticalHitChance = cboCriticalHitChance.SelectedIndex;
				oSoloHeroSelection = cboSoloHero.SelectedIndex;
				oEquipPowers = cboEquipPowers.SelectedIndex;
				oStorePrices = cboStorePrices.SelectedIndex;
				oTaloonInsanityMoves = cboTaloonInsanityMoves.SelectedIndex;
				oLevelCrit = cboLevelCrit.SelectedIndex;
				oMonsterDropChance = cboMonsterDropChance.SelectedIndex;
				oMonsterDrops = cboMonsterDrops.SelectedIndex;
			}

			oSpeedUpMusic = chkSpeedUpMusic.Checked;
			oSpeedyText = chkSpeedyText.Checked;
			oDoubleWalking = chkDoubleWalking.Checked;
			oSpeedUpBattles = chkSpeedUpBattles.Checked;

			//if (oDoubleWalking)
			//	if (MessageBox.Show(this, "WARNING:  Double walking speed will cause graphical errors with other characters, but actual gameplay should not be affected.  Continue?", "DW4 RandoHacker", MessageBoxButtons.YesNo) == DialogResult.No)
			//		return false;

			//expandRom();

			for (int lnI = 0; lnI < 8; lnI++)
				romData[0x49145 + lnI] = 0; // Make sure all characters are loaded right away; otherwise, Chapter 1 will most likely start with a ghost.

			romData[0x61c8b] = 0x07; // From awjackson, fixing the sea zone bug that mandated only zone 0 monsters in the sea.  (There should also be a zone 1)
									 // Below line:  awjackson suggestion, exposing metal slimes to fairy water, just like in the Japanese version of DW4 Randomizer.  
									 // Because of a possible event of a race between players using both the US and Japanese ROM, this will apply to all hacks.
			romData[0x4f8b0] = 0xf2;

			// Instant title screen
			romData[0x6e553] = 0x01;

			if (oSoloHero)
			{
				byte power = 0;
				power = (byte)oSoloHeroSelection;

				// Have to allow equipping of the Zenethian equipment so you can get through the tower and castle...
				romData[0x40c75 + 0x14] = (byte)Math.Pow(2, power);
				romData[0x40c75 + 0x21] = (byte)Math.Pow(2, power);
				romData[0x40c75 + 0x37] = (byte)Math.Pow(2, power);
				romData[0x40c75 + 0x44] = (byte)Math.Pow(2, power);
				romData[0x40c75 + 0x4b] = (byte)Math.Pow(2, power);

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
				romData[0x49e23] = 0x71;
				romData[0x49e24] = 0xbf;
				romData[0x4bf81] = 0x0a; // Arithmetic shift left... multipling the vitality gain by 4 instead of 2
				romData[0x4bf82] = 0x8d; // Store accumulator absolute -> 6e09
				romData[0x4bf83] = 0x09;
				romData[0x4bf84] = 0x6e;
				romData[0x4bf85] = 0x60; // end subroutine

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

				// If Mara is the solo hero, adjust Firebane to be learned at level 8.
				romData[0x4a2f2] = 0x08;
				// And adjust Blazemore to be learned at level 12.
				romData[0x4a2ef] = 0x0c;

				// If Brey is the solo hero, adjust Snowstorm to be learned at level 8.
				romData[0x4a306] = 0x08;

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

				// This will make sure that the Hero can bash the door.
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

				if (oCh4BoardingPass)
				{
					int byteToUse = (power == 0 ? 0x491a1 : power == 1 ? 0x491a9 : power == 2 ? 0x491b1 : power == 3 ? 0x491b9 : power == 4 ? 0x491c1 : power == 5 ? 0x491c9 : power == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x7a;
				}
				if (oCh4GunpowderJar)
				{
					int byteToUse = (power == 0 ? 0x491a1 : power == 1 ? 0x491a9 : power == 2 ? 0x491b1 : power == 3 ? 0x491b9 : power == 4 ? 0x491c1 : power == 5 ? 0x491c9 : power == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x70;
				}
				if (oCh5SymbolOfFaith)
				{
					int byteToUse = (power == 0 ? 0x491a1 : power == 1 ? 0x491a9 : power == 2 ? 0x491b1 : power == 3 ? 0x491b9 : power == 4 ? 0x491c1 : power == 5 ? 0x491c9 : power == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x6f;
				}
				if (oCh5GasCanister)
				{
					int byteToUse = (power == 0 ? 0x491a1 : power == 1 ? 0x491a9 : power == 2 ? 0x491b1 : power == 3 ? 0x491b9 : power == 4 ? 0x491c1 : power == 5 ? 0x491c9 : power == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x7d;
				}
			}

			int[] heroes = { 0, 1, 2, 3, 4, 5, 6, 7 };
			int finalHero = 0;
			if (oC14Random && !oSoloHero)
			{
				// Randomize the starting character for each chapter...
				// Come up with eight distinct numbers...
				for (int lnI = 0; lnI < 100; lnI++)
				{
					int numberToSwap1 = (r1.Next() % 8);
					int numberToSwap2 = (r1.Next() % 8);
					int swappy = heroes[numberToSwap1];
					heroes[numberToSwap1] = heroes[numberToSwap2];
					heroes[numberToSwap2] = swappy;
				}

				//// The hero must start Chapter 5 if Necrosaro's Mountain is replacing Aneaux.
				//// The game will crash after the Dragon rescues the hero if the Hero isn't participating in the party.
				//if (oCh5AneauxNecro)
				//{
				//	for (int lnI = 0; lnI < 7; lnI++)
				//	{
				//		if (heroes[lnI] == 0)
				//		{
				//			int numberToSwap1 = lnI;
				//			int numberToSwap2 = 4;
				//			int swappy = heroes[numberToSwap1];
				//			heroes[numberToSwap1] = heroes[numberToSwap2];
				//			heroes[numberToSwap2] = swappy;
				//		}
				//	}
				//}

				// This will set the starting hero for each chapter.
				for (int lnI = 0; lnI < 5; lnI++)
					romData[0x4914d + lnI] = (byte)(128 + heroes[lnI]);

				// If Mara is the Chapter 1 or Chapter 2-Tournament hero...
				if (heroes[0] == 2 || heroes[1] == 2)
				{
					// Adjust Firebane to be learned at level 8.
					romData[0x4a2f2] = 0x08;
					// And adjust Blazemore to be learned at level 12.
					romData[0x4a2ef] = 0x0c;
				}

				// If Brey is the Chapter 1 or Chapter 2-Tournament hero, adjust Snowstorm to be learned at level 8.
				if (heroes[0] == 3 || heroes[1] == 3)
					romData[0x4a306] = 0x08;

				// Make Taloon sane in the first 4 chapters, not just in Chapter 3.
				if (heroes[0] == 5) romData[0x47309] = romData[0x472a6] = 0;
				else if (heroes[1] == 5 || heroes[5] == 5 || heroes[6] == 5) romData[0x47309] = romData[0x472a6] = 1;
				else if (heroes[2] == 5) romData[0x47309] = romData[0x472a6] = 2;
				else romData[0x47309] = romData[0x472a6] = 3;

				// Make the Cristo and Brey join change to the randomized heroes selected in chapter 2.
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

				// Turn Nara into the other randomized hero selected in chapter 4.
				romData[0x76b3c] = (byte)heroes[7];

				finalHero = heroes[4];

				if (oCh4BoardingPass)
				{
					int byteToUse = (heroes[3] == 0 ? 0x491a1 : heroes[3] == 1 ? 0x491a9 : heroes[3] == 2 ? 0x491b1 : heroes[3] == 3 ? 0x491b9 : heroes[3] == 4 ? 0x491c1 : heroes[3] == 5 ? 0x491c9 : heroes[3] == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x7a;
				}
				if (oCh4GunpowderJar)
				{
					int byteToUse = (heroes[3] == 0 ? 0x491a1 : heroes[3] == 1 ? 0x491a9 : heroes[3] == 2 ? 0x491b1 : heroes[3] == 3 ? 0x491b9 : heroes[3] == 4 ? 0x491c1 : heroes[3] == 5 ? 0x491c9 : heroes[3] == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x70;
				}
				if (oCh5SymbolOfFaith)
				{
					int byteToUse = (heroes[4] == 0 ? 0x491a1 : heroes[4] == 1 ? 0x491a9 : heroes[4] == 2 ? 0x491b1 : heroes[4] == 3 ? 0x491b9 : heroes[4] == 4 ? 0x491c1 : heroes[4] == 5 ? 0x491c9 : heroes[4] == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x6f;
				}
				if (oCh5GasCanister)
				{
					int byteToUse = (heroes[4] == 0 ? 0x491a1 : heroes[4] == 1 ? 0x491a9 : heroes[4] == 2 ? 0x491b1 : heroes[4] == 3 ? 0x491b9 : heroes[4] == 4 ? 0x491c1 : heroes[4] == 5 ? 0x491c9 : heroes[4] == 6 ? 0x491d1 : 0x491d9);
					int lnI = 0;
					while (romData[byteToUse + lnI] != 0xff)
						lnI++;
					romData[byteToUse + lnI] = 0x7d;
				}

				// Give the Thief's Key to the Chapter 5 hero to prevent a potential unwinnable condition.
				romData[0x4911e] = (byte)heroes[4];

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

				//if (oC14Random)
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

				// Force solo Brey to solo hero in Chapter 5
				romData[0x7362a] = (byte)heroes[6];

				// Force Alena, Brey, and Cristo to solo hero in Chapter 5
				romData[0x739d9] = (byte)heroes[4];
				romData[0x739de] = (byte)heroes[5]; // <----- Give thief's key to this person...
				romData[0x739e9] = (byte)heroes[6]; // In case the player gets all three heroes at once...

				// A bunch of weird stuff preventing the Keeleon battle from occuring... (Ragnar demanding "the hero"...)
				romData[0x54e08] = (byte)heroes[0];
				romData[0x7724e] = (byte)heroes[0];
				// ... and Ragnar joining...
				romData[0x73634] = (byte)heroes[7];

				// Give the Thief's Key to the Chapter 5 hero to prevent a potential unwinnable condition.
				//romData[0x4911e] = (byte)heroes[7];
			}

			if (oRandomizeNPCs || oNPCs != 0)
			{
				heroes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

				// 0x778df (8), 0x778f1 (9), 0x778f7 (a), 0x778fd (b), 0x73678 (c), 0x7364e (d), 0x7790f (e) + 0x77573, 0x73b30 (f)
				romData[0x57169] = romData[0x57157] = 0x20; // Prevent Healie and Doran from being called a monster when talking to anyone.  Mainly in case one is acting as Panon.

				if (oRandomizeNPCs)
				{
					// Randomize NPCs
					for (int lnI = 0; lnI < 100; lnI++)
					{
						int numberToSwap1 = (r1.Next() % 8);
						int numberToSwap2 = (r1.Next() % 8);
						int swappy = heroes[numberToSwap1];
						heroes[numberToSwap1] = heroes[numberToSwap2];
						heroes[numberToSwap2] = swappy;
					}

					int[] npcs = { 0x778df, 0x778f1, 0x778f7, 0x778fd, 0x73678, 0x7364e, 0x7790f, 0x73b30 };
					int npcMark = 0;
					for (int lnI = 0; lnI < heroes.Length; lnI++)
					{
						romData[npcs[npcMark]] = (byte)(heroes[lnI] + 8);
						if (npcs[npcMark] == 0x778f7)
							romData[0x79574] = (byte)(heroes[lnI] + 8);
						if (npcs[npcMark] == 0x778fd)
							romData[0x7957c] = (byte)(heroes[lnI] + 8);
						if (npcs[npcMark] == 0x778f1 && !oSoloHero) // Make sure the correct NPC smashes the Chapter 4 Keeleon door.  Make sure you're not in solo hero mode!
							romData[0x7b399] = (byte)(heroes[lnI] + 8);
						if (npcs[npcMark] == 0x73678)
							romData[0x79597] = (byte)(heroes[lnI] + 8);
						if (npcs[npcMark] == 0x7364e)
						{
							romData[0x56c1d] = (byte)(heroes[lnI] + 8);
							romData[0x79584] = (byte)(heroes[lnI] + 8);
						}
						if (npcs[npcMark] == 0x7790f)
							romData[0x77573] = (byte)(heroes[lnI] + 8);

						npcMark++;
					}
				}

				//2016 12 15 - TheCowness
				//This checkbox is gonna scale the NPCs to the chapter they're dropped into
				//using hard-coded loadouts
				if (oNPCs == 3 || oNPCs == 4 || oNPCs == 5)
				{
					//Order if NPCs in memory is actually Healie > Orin > Laurent > Strom > Hector > Panon > Lucia > Doran (Orin before Laurent/Strom)
					int[,,] npcLoadouts = new int[8, 8, 14]{
						{//Healie
                            {//Chapter 1 (Healie)
                                18,30,35,9,20, //Agi, MP, HP1, Str, Def
                                0,0x7F, //Gold, Item (Unimportant)
                                0x25,0xA2,0x22,0x32,0x30,0x32, //Attacks
                                //Heal, Heal, Heal, Attack, Guard, Attack
                                0x00 //HP2 (Multiply by 256, add to HP1)
                            },
							{//Chapter 4 (Orin)
                                //Heal, Heal, Heal, Attack, Guard, Attack
                                40,42,45,30,40, 0,0x7F, 0x25,0x22,0x22,0x32,0x30,0x32, 0x00
							},
							{//Chapter 3 (Laurent)
                                //Heal, Heal, Heal, ParalyzeHit, Guard, Attack
                                25,30,35,20,20, 0,0x7F, 0x25,0x22,0x22,0x37,0x30,0x32, 0x00
							},
							{//Chapter 3 (Strom)
                                //Heal, Heal, Heal, Attack, Guard, Attack
                                18,30,35,15,30, 0,0x7F, 0x25,0x22,0x22,0x32,0x30,0x32, 0x00
							},
							{//Chapter 5 (Hector)
                                //Heal, Heal, Heal, Attack, Qrt Guard, Attack
                                40,42,45,30,40, 0,0x7F, 0x25,0x22,0x22,0x32,0x31,0x32, 0x00
							},
							{//Chapter 5 (Panon)
                                //Healmore, Healmore, Healmore, SleepHit, Dazed, SleepHit
                                65,90,80,45,60, 0,0x7F, 0x26,0x26,0x26,0x35,0x2C,0x35, 0x00
							},
							{//Chapter 5 (Lucia)
                                //HealAll, HealUs, HealUs, SleepHit, Upper, Upper
                                95,210,145,75,90, 0,0x7F, 0x27,0x28,0x28,0x35,0x1C,0x1C, 0x00
							},
							{//Chapter 5 (Doran)
                                //HealAll, HealUs, HealUsAll, Attack, Increase, Increase
                                110,255,165,95,110, 0,0x7F, 0x27,0x28,0x29,0x32,0x1D,0x1D, 0x00
							}
						},
						{//Orin
                            {//Chapter 1 (Healie)
                                4,0,60,50,30, 0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 4 (Orin)
                                8,0,82,58,33, 0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 3 (Laurent)
                                //Paralyzehitx2, Attack x4
                                6,0,70,60,30, 0,0x7F,0x37,0x37,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 3 (Strom)
                                6,0,95,82,43, 0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 5 (Hector)
                                //Attack x6
                                20,0,90,95,40, 0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 5 (Panon)
                                //Sleephitx3, Attack x3
                                35,0,120,150,80, 0,0x7F,0x35,0x35,0x35,0x32,0x32,0x32,0x00
							},
							{//Chapter 5 (Lucia)
                                //Attack x6
                                50,0,5,200,100, 0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x01
							},
							{//Chapter 5 (Doran)
                                //Attack x6
                                55,0,95,250,120, 0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x01
							}
						},
						{//Laurent
                            {//Chapter 1 (Healie)
                                //Heal, Heal, Blaze, Sleepmore, ParalyzeHit, Heal
                                15,40,40,28,25,0,0x7F,0xA5,0x22,0x00,0x11,0x37,0x25,0x00
							},
							{//Chapter 4 (Orin)
                                //Heal, Heal, Firebal, Sleep, ParalyzeHit, Heal
                                25,68,58,35,30,0,0x7F,0xA5,0x22,0x03,0x10,0x37,0x25,0x00
							},
							{//Chapter 3 (Laurent)
                                //Heal, Heal, Firebal, Sleep, ParalyzeHit, Heal
                                18,68,58,28,25,0,0x7F,0xA5,0x22,0x03,0x10,0x37,0x25,0x00
							},
							{//Chapter 3 (Strom)
                                //Firebal, Firebal, Firebal, Sleep, ParalyzeHit, Heal
                                18,68,58,28,25,0,0x7F,0x03,0x03,0x03,0x10,0x37,0x25,0x00
							},
							{//Chapter 5 (Hector)
                                //Heal, Heal, Firebal, Sleep, ParalyzeHit, Heal
                                25,68,58,35,30,0,0x7F,0xA5,0x22,0x03,0x10,0x37,0x25,0x00
							},
							{//Chapter 5 (Panon)
                                //Healmore, Healmore, Firebane, Sleep, ParalyzeHit, Healmore
                                45,90,75,45,50,0,0x7F,0x26,0x26,0x04,0x10,0x37,0x26,0x00
							},
							{//Chapter 5 (Lucia)
                                //HealAll, HealAll, Firevolt, Sleep, ParalyzeHit, HealAll
                                70,180,135,80,80,0,0x7F,0x27,0x27,0x05,0x10,0x37,0x27,0x00
							},
							{//Chapter 5 (Doran)
                                //HealAll, Firevolt, Blazemost, Blizzard, Sap, HealAll
                                80,255,160,90,100,0,0x7F,0x27,0x05,0x02,0x09,0x16,0x27,0x00
							}
						},
						{//Strom
                            {//Chapter 1 (Healie)
                                10,0,60,50,30,0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 4 (Orin)
                                //25% Crit x2, Attack x4
                                25,0,95,82,43,0,0x7F,0x33,0x33,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 3 (Laurent)
                                //Sleephit, Paralyzehit, Attack x4
                                25,0,80,60,30,0,0x7F,0x35,0x37,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 3 (Strom)
                                18,0,95,82,43,0,0x7F,0x32,0x32,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 5 (Hector)
                                //25% Crit x2, Attack x4
                                25,0,95,82,43,0,0x7F,0x33,0x33,0x32,0x32,0x32,0x32,0x00
							},
							{//Chapter 5 (Panon)
                                //Sleephitx2, Flusteredx1, Attack x3
                                50,0,150,100,80,0,0x7F,0x35,0x35,0x2B,0x32,0x32,0x32,0x00
							},
							{//Chapter 5 (Lucia)
                                //25% Crit x2, Attack x4
                                60,0,45,150,120,0,0x7F,0x33,0x33,0x32,0x32,0x32,0x32,0x01
							},
							{//Chapter 5 (Doran)
                                //25% Crit x6
                                75,0,145,200,150,0,0x7F,0x33,0x33,0x33,0x33,0x33,0x33,0x01
							}
						},
						{//Hector
                            {//Chapter 1 (Healie)
                                //25% Crit x2, Attack x2, Build Up x2
                                15,0,60,50,30,0,0x7F,0x33,0x33,0x32,0x32,0x2D,0x2D,0x00
							},
							{//Chapter 4 (Orin)
                                //25% Crit x2, Attack x2, Build Up x2
                                20,0,96,50,40,0,0x7F,0x33,0x33,0x32,0x32,0x2D,0x2D,0x00
							},
							{//Chapter 3 (Laurent)
                                //25% Crit x2, Attack x2, Build Up x2
                                20,0,80,60,30,0,0x7F,0x33,0x33,0x32,0x32,0x2D,0x2D,0x00
							},
							{//Chapter 3 (Strom)
                                //25% Crit x2, Attack x2, Build Up x2
                                15,0,95,82,43,0,0x7F,0x33,0x33,0x32,0x32,0x2D,0x2D,0x00
							},
							{//Chapter 5 (Hector)
                                //25% Crit x2, Build Up x2, Attack x2
                                26,0,96,58,47,0,0x7F,0xB2,0x33,0x2D,0x32,0x33,0x2D,0x00
							},
							{//Chapter 5 (Panon)
                                //25% Crit x2, Attack x2, Build Up x2
                                40,0,120,80,70,0,0x7F,0xB2,0x33,0x2D,0x32,0x33,0x2D,0x00
							},
							{//Chapter 5 (Lucia)
                                //25% Crit x2, Attack x2, Build Up x2
                                50,0,45,150,100,0,0x7F,0xB2,0x33,0x2D,0x32,0x33,0x2D,0x01
							},
							{//Chapter 5 (Doran)
                                //25% Crit x2, Attack x2, Build Up x2
                                60,0,105,180,120,0,0x7F,0xB2,0x33,0x2D,0x32,0x33,0x2D,0x01
							}
						},
						{//Panon
                            {//Chapter 1 (Healie)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                10,12,40,40,15,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							},
							{//Chapter 4 (Orin)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                25,18,75,75,30,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							},
							{//Chapter 3 (Laurent)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                25,18,60,60,20,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							},
							{//Chapter 3 (Strom)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                18,18,60,60,20,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							},
							{//Chapter 5 (Hector)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                25,18,60,60,35,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							},
							{//Chapter 5 (Panon)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                38,24,85,88,53,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							},
							{//Chapter 5 (Lucia)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                60,30,160,120,80,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							},
							{//Chapter 5 (Doran)
                                //Strange Dance, Attack, SleepHit, Sleep, Attack, SleepHit
                                75,30,200,150,100,0,0x7F,0x39,0x32,0x35,0x10,0x32,0x35,0x00
							}
						},
						{//Lucia
                            {//Chapter 1 (Healie)
                                15,40,40,30,20, //Agi, MP, HP1, Str, Def
                                0,0x7F, //Gold, Item (Unimportant)
                                0x25,0xA2,0x32,0x17,0x30,0x13, //Attacks
                                //Heal, Heal, Attack, Defense, Defend, Surround
                                0x00 //HP2 (Multiply by 256, add to HP1)
                            },
							{//Chapter 4 (Orin)
                                //Heal, Heal, Attack, Defense, Defend, Surround
                                40,42,45,50,40,0,0x7F,0x25,0x22,0x32,0x17,0x30,0x13,0x00
							},
							{//Chapter 3 (Laurent)
                                //Heal, Heal, Attack, Defense, Defend, Surround
                                25,30,35,30,20,0,0x7F,0x25,0x22,0x32,0x17,0x30,0x13,0x00
							},
							{//Chapter 3 (Strom)
                                //Heal, Heal, Attack, Defense, Defend, Surround
                                18,30,35,35,30,0,0x7F,0x25,0x22,0x32,0x17,0x30,0x13,0x00
							},
							{//Chapter 5 (Hector)
                                //Heal, Heal, Attack, Defense, Defend, Surround
                                40,42,45,50,40,0,0x7F,0x25,0x22,0x32,0x17,0x30,0x13,0x00
							},
							{//Chapter 5 (Panon)
                                //Healmore, Healmore, Attack, Defense, Defend, Surround
                                65,90,80,70,60,0,0x7F,0x26,0x26,0x32,0x17,0x30,0x13,0x00
							},
							{//Chapter 5 (Lucia)
                                //HealAll, HealAll, Attack, Defense, Defend, Surround
                                80,180,156,95,81,0,0x7F,0x27,0xA4,0x32,0x17,0x30,0x13,0x00
							},
							{//Chapter 5 (Doran)
                                //HealAll, HealAll, Attack, Defense, Defend, Surround
                                90,210,180,110,90,0,0x7F,0x27,0xA4,0x32,0x17,0x30,0x13,0x00
							}
						},
						{//Doran
                            {//Chapter 1 (Healie)
                                //Fire Breath, 25% Crit, Fire Breath, 25% Crit, Fire Breath, Sweet Breath
                                5,0,50,30,25,0,0x7F,0x3C,0x33,0x3C,0x33,0x3C,0x42,0x00
							},
							{//Chapter 4 (Orin)
                                //Freezing Wind, 25% Crit, Freezing Wind, 25% Crit, Freezing Wind, Sweet Breath
                                15,0,100,90,40,0,0x7F,0x3F,0x33,0x3F,0x33,0x3F,0x42,0x00
							},
							{//Chapter 3 (Laurent)
                                //Freezing Wind, 25% Crit, Freezing Wind, 25% Crit, Sweet Breath, Sweet Breath
                                10,0,75,60,30,0,0x7F,0x3F,0x33,0x3F,0x33,0x42,0x42,0x00
							},
							{//Chapter 3 (Strom)
                                //Fire Breath, 25% Crit, Fire Breath, 25% Crit, Fire Breath, Sweet Breath
                                10,0,85,70,30,0,0x7F,0x3C,0x33,0x3C,0x33,0x3C,0x42,0x00
							},
							{//Chapter 5 (Hector)
                                //Freezing Wind, 25% Crit, Freezing Wind, 25% Crit, Freezing Wind, Sweet Breath
                                15,0,100,80,50,0,0x7F,0x3F,0x33,0x3F,0x33,0x3F,0x42,0x00
							},
							{//Chapter 5 (Panon)
                                //Scorching Gas, 25% Crit, Scorching Gas, 25% Crit, Sweet Breath, Sweet Breath
                                20,0,150,150,100,0,0x7F,0x3D,0x33,0x3D,0x33,0x42,0x42,0x00
							},
							{//Chapter 5 (Lucia)
                                //Violent Blaze, 25% Crit, Violent Blaze, 25% Crit, Violent Blaze, Sweet Breath
                                30,0,250,180,140,0,0x7F,0x3D,0x33,0x3D,0x33,0x3D,0x42,0x00
							},
							{//Chapter 5 (Doran)
                                //Blizzard Breath, 25% Crit, Blizzard Breath, 25% Crit, Blizzard Breath, Sweet Breath
                                35,0,2,195,160,0,0x7F,0x40,0x33,0x40,0x33,0x40,0x42,0x01
							}
						}
					};
					//Loop through all of the heroes again
					for (int lnI = 0; lnI < heroes.Length; lnI++)
					{
						//lnI is where they're getting dropped, heroes[lnI] is which character we're dropping in.
						for (int lnJ = 0; lnJ < 14; lnJ++)
						{
							//The +2 here is because we're skipping the EXP bytes and starting at Agi
							romData[0x60056 + ((heroes[lnI] + 197) * 22) + lnJ + 2] = (byte)npcLoadouts[heroes[lnI], lnI, lnJ];
						}
					}
				}

				// Randomize stats
				if (oNPCs == 1 || oNPCs == 2 || oNPCs == 4 || oNPCs == 5)
				{
					for (int lnI = 0; lnI < heroes.Length; lnI++)
					{
						//lnI is where they're getting dropped, heroes[lnI] is which character we're dropping in.
						for (int lnJ = 0; lnJ < 5; lnJ++)
						{
							int stat = romData[0x60056 + ((heroes[lnI] + 197) * 22) + lnJ + 2];
							stat = ScaleValue(stat, (oNPCs == 1 || oNPCs == 4) ? 2.0 : 4.0, 1.0, r1);
							//The +2 here is because we're skipping the EXP bytes and starting at Agi
							romData[0x60056 + ((heroes[lnI] + 197) * 22) + lnJ + 2] = (byte)stat;
						}
					}
				}
				else if (oNPCs == 6)
				{
					for (int lnI = 0; lnI < heroes.Length; lnI++)
					{
						//lnI is where they're getting dropped, heroes[lnI] is which character we're dropping in.
						for (int lnJ = 0; lnJ < 5; lnJ++)
						{
							int stat = inverted_power_curve((lnJ == 2 ? 16 : 1), (lnJ == 2 ? 400 : 255), 1, 3.0, r1)[0];
							//The +2 here is because we're skipping the EXP bytes and starting at Agi
							romData[0x60056 + ((heroes[lnI] + 197) * 22) + lnJ + 2] = (byte)stat;
						}
						for (int lnJ = 7; lnJ < 13; lnJ++)
						{
							int attack;
							if (r1.Next() % 2 == 1)
								attack = (byte)(r1.Next() % 0x67);
							else
								attack = 0x32;
							if (attack == 0x15 || attack == 0x55)
								lnJ--; // redo randomization.  Transform and super slime is a bad idea to use here.

							//The +2 here is because we're skipping the EXP bytes and starting at Agi
							romData[0x60056 + ((heroes[lnI] + 197) * 22) + lnJ + 2] = (byte)attack;
						}
					}
				}
			}

			chapter1Adjustments(r1);
			chapter2Adjustments();
			chapter3Adjustments();
			chapter4Adjustments();
			chapter5Adjustments();

			criticalHits(r1);
			taloonInsanity(r1);
			if (oRandomizeMap)
				// If the map is forced to revert to the original map, revert the Ch2 and Ch5 town warp locations.
				if (!randomizeMapv5(r1))
				{
					romData[0x3be1c + (3 * 14) + 0] = 0x47;
					romData[0x3be1c + (3 * 14) + 1] = 0x39;
					romData[0x3be1c + (3 * 43) + 0] = 0xb2;
					romData[0x3be1c + (3 * 43) + 1] = 0x4d;
				}
			//swapMonsters(r1);
			randomizeMonsterStats(r1);
			randomizeMonsterDrops(r1);
			randomizeMonsterAttacks(r1);
			randomizeMonsterResistances(r1);
			randomizeTreasures(r1);

			int xpBoost = oXPBoost;
			int xpRandom = oXPRandom;

			// Now adjust XP for all monsters...
			for (int lnI = 0; lnI <= 0xc2; lnI++)
			{
				double xp = (romData[0x60056 + (lnI * 22) + 1] * 256) + romData[0x60056 + (lnI * 22) + 0];
				double origXP = xp;
				if (xpRandom != 5)
				{
					xp *= xpBoost == 0 ? 0 : xpBoost == 1 ? .5 : xpBoost == 2 ? 1.0 : xpBoost == 3 ? 1.5 : xpBoost == 4 ? 2.0 : xpBoost == 5 ? 3.0 : xpBoost == 6 ? 4.0 : 5.0;

					xp = ScaleValueDouble(xp, xpRandom == 0 ? 1.0 : xpRandom == 1 ? 1.5 : xpRandom == 2 ? 2.0 : xpRandom == 3 ? 3.0 : 4.0, 1.0, r1, false);
				}
				else
				{
					xp = inverted_power_curve(1, (int)(12000 * (xpBoost == 0 ? 0 : xpBoost == 1 ? .5 : xpBoost == 2 ? 1.0 : xpBoost == 3 ? 1.5 : xpBoost == 4 ? 2.0 : xpBoost == 5 ? 3.0 : xpBoost == 6 ? 4.0 : 5.0)), 1, 0.05, r1)[0];
				}

				int xpTrue = (int)Math.Round(xp);
				if (xpTrue < 1 && origXP >= 1) xpTrue = 1;
				if (xpTrue > 65000) xpTrue = 65000;
				romData[0x60056 + (lnI * 22) + 1] = (byte)(xpTrue / 256);
				romData[0x60056 + (lnI * 22) + 0] = (byte)(xpTrue % 256);
			}

			xpBoost = oGoldBoost;
			xpRandom = oGoldRandom;

			// Then the gold for all monsters...
			for (int lnI = 0; lnI <= 0xc2; lnI++)
			{
				double xp = ((romData[0x60056 + (lnI * 22) + 18] % 4) * 256) + (romData[0x60056 + (lnI * 22) + 7]);
				double origXP = xp;

				if (xpRandom != 5)
				{
					xp *= xpBoost == 0 ? 0 : xpBoost == 1 ? .5 : xpBoost == 2 ? 1.0 : xpBoost == 3 ? 1.5 : xpBoost == 4 ? 2.0 : xpBoost == 5 ? 3.0 : xpBoost == 6 ? 4.0 : 5.0;

					xp = ScaleValueDouble(xp, xpRandom == 0 ? 1.0 : xpRandom == 1 ? 1.5 : xpRandom == 2 ? 2.0 : xpRandom == 3 ? 3.0 : 4.0, 1.0, r1, false);
				}
				else
				{
					xp = inverted_power_curve(1, (int)(200 * (xpBoost == 0 ? 0 : xpBoost == 1 ? .5 : xpBoost == 2 ? 1.0 : xpBoost == 3 ? 1.5 : xpBoost == 4 ? 2.0 : xpBoost == 5 ? 3.0 : xpBoost == 6 ? 4.0 : 5.0)), 1, 0.1, r1)[0];
				}

				int xpTrue = (int)Math.Round(xp);
				if (xpTrue < 1 && origXP >= 1) xpTrue = 1;
				if (xpTrue > 1000) xpTrue = 1023;
				romData[0x60056 + (lnI * 22) + 18] -= (byte)(romData[0x60054 + (lnI * 22) + 20] % 4);
				romData[0x60056 + (lnI * 22) + 18] += (byte)(xpTrue / 256);
				romData[0x60056 + (lnI * 22) + 7] = (byte)(xpTrue % 256);
			}


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
			// NOTE:  RNG @ 0012
			for (int lnI = 0; lnI < 8; lnI++)
			{
				double encounterRate = (romData[0x6228b + lnI]);
				if (oEncounterRate == 0) encounterRate = Math.Round(encounterRate / 4);
				if (oEncounterRate == 1) encounterRate = Math.Round(encounterRate * 3 / 8);
				if (oEncounterRate == 2) encounterRate = Math.Round(encounterRate / 2);
				if (oEncounterRate == 3) encounterRate = Math.Round(encounterRate * 3 / 4);
				if (oEncounterRate == 5) encounterRate = Math.Round(encounterRate * 3 / 2);
				if (oEncounterRate == 6) encounterRate = Math.Round(encounterRate * 2);
				if (oEncounterRate == 7)
				{
					encounterRate = r1.Next() % 32;
					if (encounterRate > 16)
						encounterRate = r1.Next() % 32;

					if (encounterRate < 4)
						encounterRate = 4;
				}
				romData[0x6228b + lnI] = (byte)encounterRate;
			}

			for (int lnI = 0; lnI < 16; lnI++)
			{
				double encounterRate = (romData[0x62350 + lnI]);
				if (oEncounterRate == 0) encounterRate = Math.Round(encounterRate / 4);
				if (oEncounterRate == 1) encounterRate = Math.Round(encounterRate * 3 / 8);
				if (oEncounterRate == 2) encounterRate = Math.Round(encounterRate / 2);
				if (oEncounterRate == 3) encounterRate = Math.Round(encounterRate * 3 / 4);
				if (oEncounterRate == 5) encounterRate = Math.Round(encounterRate * 3 / 2);
				if (oEncounterRate == 6) encounterRate = Math.Round(encounterRate * 2);
				if (oEncounterRate == 7)
				{
					encounterRate = r1.Next() % 32;
					if (encounterRate > 16)
						encounterRate = r1.Next() % 32;

					if (encounterRate < 4)
						encounterRate = 4;
				}
				romData[0x62350 + lnI] = (byte)encounterRate;
			}

			randomizeEquipmentPowers(r1);
			randomizeHeroEquipment(r1);
			randomizeMonsterZones(r1);
			randomizeStores(r1);
			randomizeHeroStats(r1);
			randomizeHeroSpells(r1);
			randomizeNecrosaro(r1);

			if (oSpeedUpBattles)
			{
				speedUpBattles();
			}

			if (oSpeedUpMusic)
			{
				speedUpMusic();
			}

			if (oSpeedyText)
				speedyText();

			if (oDoubleWalking)
			{
				romData[0x788ec] = romData[0x7cb08] = romData[0x7a5f3] = romData[0x3cb07] = 0x20;
				romData[0x788e7] = romData[0x7a5ee] = romData[0x7cb02] = romData[0x3cb02] = romData[0x769ae] = 0x02;
				romData[0x3cafa] = romData[0x3cafb] = romData[0x3cafc] = romData[0x3cafd] = romData[0x3cafe] = romData[0x3caff] = romData[0x3cb00] = 0xea;
				romData[0x74b32] = romData[0x74b33] = romData[0x74b34] = romData[0x74b35] = romData[0x74b36] = romData[0x74b37] = romData[0x74b38] = 0xea;
				romData[0x61bf6] = romData[0x61bf7] = romData[0x61bf8] = romData[0x61bf9] = romData[0x61bfa] = romData[0x61bfb] = romData[0x61bfc] = 0xea;
				// If this isn't put in here, the game locks after dying and reviving at the House Of Healing.
				romData[0x7cafa] = romData[0x7cafb] = romData[0x7cafc] = romData[0x7cafd] = romData[0x7cafe] = 0xea;
				// Doubles the speed of the NPCs.  Required to get past the offering challenge in Chapter 2, at a minimum.
				romData[0x3da5b] = 0xc0;
				// Makes sure the ship goes double speed at the Cave of the Silver Statuette in Chapters 3 and 5.
				romData[0x72012] = 0x41;
				romData[0x72016] = 0x42;
				romData[0x7201a] = 0x41;
				romData[0x71fdd] = 0x41;
				romData[0x174ed] = romData[0x1750a] = 0x5f;
				romData[0x71fa1] = 0x41;
				romData[0x71fa9] = 0x41;
				romData[0x71fb9] = 0x41;
				romData[0x71fbd] = 0x41;
				romData[0x71fc5] = 0x41;
				romData[0x71fe1] = 0x41;
				romData[0x71fe6] = 0x41; // Riverton
				romData[0x72bdc] = 0x41; // Final Cave ship
										 // Prevents a slow down as you escape Keeleon in Chapter 4.
				romData[0x76c4b] = 0x02;
			}

			saveRom(true);

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
						}
						else
						{
							char character = Convert.ToChar(name.Substring(lnJ, 1).ToLower());
							byteArray.Add(character - 97);
						}
					}
					else
					{
						char character = Convert.ToChar(name.Substring(lnJ, 1).ToLower());
						byteArray.Add(character - 97);
					}
				}
				romData[0x2fba0 + stringMarker] = (byte)byteArray.ToArray().Length;
				stringMarker++;
				foreach (int byteSingle in byteArray)
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

		private void swapMonsters(Random r1)
		{
			// First, let's shuffle boss fights if selected
			if (oSwapBossStats)
			{
				byte[] bosses = new byte[] { 0xb3, 0x12, 0xa4, 0xb0, 0xb1, 0xb2, 0xb4, 0xba, 0xc0, 0xbf, 0xbe, 0xb5, 0xc2, 0xc1, 0xbc, 0xb6, 0xb7, 0xb8, 0xb9, 0x91, 0x59, 0x62 };
				bosses = shuffle(bosses, r1);
				for (int lnI = 0; lnI < bosses.Length; lnI++)
				{
					int byteToUse = 0x6235c + (8 * lnI);
					for (int lnJ = 0; lnJ < 4; lnJ++)
						romData[byteToUse + lnJ] = 0xff;
					for (int lnJ = 5; lnJ < 8; lnJ++)
						romData[byteToUse + lnJ] = 0x00;


				}
			}

			if (oSwapMonsterStats)
			{

			}
		}

		private byte[] shuffle(byte[] treasureData, Random r1)
		{
			return treasureData;
		}

		private void swap(int firstAddress, int secondAddress)
		{
			byte holdAddress = romData[secondAddress];
			romData[secondAddress] = romData[firstAddress];
			romData[firstAddress] = holdAddress;
		}

		private void expandRom()
		{
			byte[] newRom = new byte[romData.Length + (32 * 16384)];
			for (int lnI = 0; lnI < romData.Length; lnI++)
				newRom[lnI] = romData[lnI];

			romData = newRom;

			romData[0x4] = 0x40;
			for (int lnI = 0x7c010; lnI < 0x80010; lnI++)
			{
				romData[lnI + 0x40000] = romData[lnI + 0x80000] = romData[lnI];
			}
			romData[0x3ffb0] = romData[0x7ffb0] = romData[0xbffb0] = romData[0xfffb0] = 0x30;
		}

		private void chapter1Adjustments(Random r1)
		{
			if (oCh1InstantWell || oChapter5Start)
			{
				romData[0x23668] = 0x00;
				romData[0x23669] = 0xf0;
			}

			if (oCh1FlyingShoes)
			{
				romData[0x491d3] = 0x6c;
			}

			if (oCh1Moat && !oRandomizeMap)
			{
				romData[0x2cea8] = 0x84;
			}

			// Now to adjust the bonus for Chapter 1
			int xpBoost = oXPBoost;
			int xpRandom = oXPRandom;
			int xpBonus = xpBoost == 0 ? 0 : xpBoost == 1 ? 5 : xpBoost == 2 ? 10 : xpBoost == 3 ? 15 : xpBoost == 4 ? 20 : xpBoost == 5 ? 30 : xpBoost == 6 ? 40 : 50;
			xpBonus = ScaleValue(xpBonus, xpRandom == 0 ? 1.0 : xpRandom == 1 ? 1.5 : xpRandom == 2 ? 2.0 : xpRandom == 3 ? 3.0 : 4.0, 1.0, r1, false);
			romData[0x765ee] = (byte)xpBonus;
		}

		private void chapter2Adjustments()
		{
			if (oCh2InstantWallKick)
			{
				romData[0x72113] = 0xea;
				romData[0x72114] = 0xea;
			}

			if (oCh2EndorEntry || oChapter5Start)
			{
				romData[0x72d8d] = 0x82; // This actually is a Chapter 1 trigger which must be triggered currently, but the soldier blocking is an AND 0x02.
			}

			// Make Chapter 2 adjustments if requested.
			if (oCh2AwardXPTournament)
			{
				romData[0x60054 + (0xaf * 22) + 2] = 60;
				romData[0x60054 + (0xb0 * 22) + 2] = 80;
				romData[0x60054 + (0xb1 * 22) + 2] = 80;
				romData[0x60054 + (0xb2 * 22) + 2] = 100;
				romData[0x60054 + (0xba * 22) + 2] = 100;
			}
		}

		private void chapter3Adjustments()
		{
			if (oCh3BuildBridges)
			{
				romData[0x200bf] = 0x00;
				romData[0x200c0] = 0xf0;
			}

			if (oCh3BuildTunnel || oChapter5Start)
			{
				romData[0x22fce] = 0x00;
				romData[0x22fcf] = 0xd0;
			}

			// Make Chapter 3 adjustments if requested.
			// Make the shop one piece of gold... I still think that Chapter 3 sucks.  :)
			if (oCh3Shop1)
			{
				romData[0x5603c] = 0x00;
				romData[0x56044] = 0x01;
			}
			if (oCh3Shop25K)
			{
				romData[0x5603c] = 0x61;
				romData[0x56044] = 0xa8;
			}
			// This can make the tunnel one piece of gold...
			if (oCh3Tunnel1)
			{
				romData[0x56641] = 0x00;
				romData[0x56645] = 0x01;
			}

			// Sell all items from the store by not doing the comparison.
			romData[0x55fb0] = 0xea;
			romData[0x55fb1] = 0xea;
		}

		private void chapter4Adjustments()
		{
			if (oCh4BoardingPass && !oC14Random && !oSoloHero)
			{
				int byteToUse = 0x491ba;
				romData[byteToUse] = 0x7a;
			}
			if (oCh4GunpowderJar && !oC14Random && !oSoloHero)
			{
				int byteToUse = 0x491bb;
				romData[byteToUse] = 0x70;
			}

			romData[0x5499e] = 0xff; // Skip the talking to the one person at the bottom of the ship.
		}

		private void chapter5Adjustments()
		{
			if (oCh5SymbolOfFaith && !oC14Random && !oSoloHero)
			{
				int byteToUse = 0x491a1; // Hero
				int lnI = 0;
				while (romData[byteToUse + lnI] != 0xff)
					lnI++;
				romData[byteToUse + lnI] = 0x6f;
			}
			if (oCh5GasCanister && !oC14Random && !oSoloHero)
			{
				int byteToUse = 0x491a1; // Hero
				int lnI = 0;
				while (romData[byteToUse + lnI] != 0xff)
					lnI++;
				romData[byteToUse + lnI] = 0x7d;
			}

			if (oCh5BlowUpHometown)
			{
				romData[0x23494] = 0xea;
				romData[0x23495] = 0xea;
			}

			if (oInstantFinalCave)
				romData[0x2ea19] = 0x85;

			if (oCh5AneauxNecro)
			{
				romData[0x2385a] = 0xe0;
				romData[0x2385b] = 0x15;
				romData[0x2385c] = 0x25;
				romData[0x2385d] = 0x14;
				romData[0x3be57] = 0x3c;
			}

			if (oCh5NoZGear)
			{
				romData[0x17634] = 0x14; // Instead of 0x13, the check at the tower
				romData[0x23773] = 0xf0; // Force a skip of a Zenithian Gear check to enter the castle itself.
				romData[0x23774] = 0x34; // BEQ +34 instead
			}

			if (oChapter5Start)
			{
				// Make sure the Branca tunnel is built.
				romData[0x22fce] = 0x00;
				romData[0x22fcf] = 0xd0;
				// Make sure Loch Tower is in the correct state.  You can "beat Chapter 1" otherwise, which disables all encounters and does a lot of weird stuff.
				// Make sure Santeem is in the correct state.  The monsters won't be there in floor one, but Balzack would... pre-Keeleon.
				romData[0x7215a] = 0x06;
				romData[0x7215c] = 0x20;

				// This code will get us starting in Chapter 5.
				romData[0x4abb4] = 0x20;
				romData[0x4abb5] = 0x68;
				romData[0x4abb6] = 0xbf;

				romData[0x4abc3] = 0xea;
				romData[0x4abc4] = 0xea;

				romData[0x4bf78] = 0xa9; // LDA #$04
				romData[0x4bf79] = 0x04;
				romData[0x4bf7a] = 0x8d; // STA $615A
				romData[0x4bf7b] = 0x5a;
				romData[0x4bf7c] = 0x61;
				romData[0x4bf7d] = 0xa2; // Carry over from what was replaced
				romData[0x4bf7e] = 0x08;
				romData[0x4bf7f] = 0x8a;
				romData[0x4bf80] = 0x60; // JSR

				for (int lnI = 0; lnI < 8; lnI++)
					romData[0x49145 + lnI] = 4; // Make sure all characters are loaded in Chapter 4 to avoid ghosting.
			}

			// Give full control over all players in Chapter 5.  You lose the wagon control though.  I would LOVE to figure out how to get both though!  Maybe some nops?
			if (oCh5ControlAllChars)
				romData[0x46e1e] = 0x7f; // You can make it any number higher than 04, chapter 5 I think... 
		}

		private void taloonInsanity(Random r1)
		{
			if (oTaloonInsanity == 1) // Always insane
			{
				romData[0x472a7] = romData[0x472a8] = 0xea; // Skip the comparison
			} else if (oTaloonInsanity == 2) // Never insane
			{
				romData[0x472a5] = 0x4c; // JMP $B2F4, meaning he's not insane.
				romData[0x472a6] = 0xf4;
				romData[0x472a7] = 0xb2;
			}
			// .SelectedIndex = 0 is not neccessary; it's covered earlier.  (possible hero randomization)
			romData[0x472ad] = (byte)(oTaloonInsanityChance == 0 ? 0x7f : oTaloonInsanityChance == 1 ? 0x3f : oTaloonInsanityChance == 2 ? 0x1f :
				oTaloonInsanityChance == 3 ? 0x0f : oTaloonInsanityChance == 4 ? 0x07 : oTaloonInsanityChance == 5 ? 0x03 : oTaloonInsanityChance == 6 ? 0x01 : 0x00);

			if (oTaloonInsanityMoves >= 1)
			{
				byte[] badMoves = { 0x3c, 0x3d, 0x4d, 0x50, 0x53, 0x56, 0x57, 0x59, 0x5a, 0x68,
									0x73, 0x75, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7f, 0x82,
									0x85, 0x86, 0x87, 0x89, 0x8a, 0x8b, 0x8d, 0x8e, 0x90, 0x91,
									0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x9b, 0x9c, 0x9e, 0xa0,
									0xa3, 0xa5, 0xa6, 0xa8, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xb0,
									0xb1, 0xb2, 0xb3, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb,
									0xbc, 0xbd, 0xbf, 0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6,
									0xc7, 0xc8, 0xc9, 0xcb, 0xcc, 0xcd, 0xce, 0xcf, 0xd0, 0xd1,
									0xd2, 0xd6, 0xd7, 0xd8, 0xd9, 0xdb, 0xdc, 0xe1, 0xe4, 0xe6,
									0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xef, 0xf3, 0xf4,
									0xf5, 0xf7, 0xf8, 0xf9, 0xfa, 0xfc, 0xfe }; // 107
				byte[] goodMoves = { 0x4e, 0x4f, 0x51, 0x52, 0x55, 0x67, 0x6d, 0x6e, 0x6f, 0x70,
									 0x71, 0x74, 0x76, 0x7d, 0x7e, 0x80, 0x83, 0x84, 0x8c, 0x8f,
									 0xa1, 0xa2, 0xa4, 0xa9, 0xbe, 0xca, 0xd4, 0xd5, 0xdd, 0xdf,
									 0xe0, 0xe2, 0xe3, 0xe5, 0xee, 0xf0, 0xf1, 0xf2, 0xf6, 0xfd,
									 0x3e }; // 41
				byte[] excellentMoves = { 0x4f, 0x52, 0x6e, 0x7d, 0x80, 0xa1, 0xa2, 0xa4, 0xa9, 0xdd, 0xdf, 0xe0, 0xe2, 0xe3, 0xf1, 0x3e }; // 16
				byte[] whoaMoves = { 0xa9, 0xa9, 0x7d, 0x7d, 0xa9, 0xa9, 0x7d, 0x7d, 0x45, 0x3e }; // 10

				for (int lnI = 0; lnI < 13; lnI++)
				{
					if (oTaloonInsanityMoves == 1 || (oTaloonInsanityMoves == 2 && r1.Next() % 4 != 0) || (oTaloonInsanityMoves == 3 && r1.Next() % 2 == 0) || (oTaloonInsanityMoves == 4 && r1.Next() % 4 == 0))
						romData[0x47368 + lnI] = badMoves[r1.Next() % badMoves.Length];
					else if (oTaloonInsanityMoves <= 5)
						romData[0x47368 + lnI] = goodMoves[r1.Next() % goodMoves.Length];
					else if (oTaloonInsanityMoves == 6)
						romData[0x47368 + lnI] = excellentMoves[r1.Next() % excellentMoves.Length];
					else if (oTaloonInsanityMoves == 7)
						romData[0x47368 + lnI] = whoaMoves[r1.Next() % whoaMoves.Length];
				}
			}
		}

		private void criticalHits(Random r1)
		{
			if (oLevelCrit == 1)
				romData[0x457cd] = (byte)(r1.Next() % 8);
			if (oLevelCrit == 2)
				romData[0x457cd] = 0x56; // Impossible to get this character number
			if (oLevelCrit == 3)
				romData[0x457ce] = romData[0x457cf] = 0xea;

			romData[0x457c3] = (byte)(oCriticalHitChance == 0 ? 0 :
								oCriticalHitChance == 1 ? 4 :
								oCriticalHitChance == 2 ? 8 :
								oCriticalHitChance == 3 ? 16 :
								oCriticalHitChance == 4 ? 32 :
								oCriticalHitChance == 5 ? 64 :
								oCriticalHitChance == 6 ? 128 : 255);
		}

		private void speedUpBattles()
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

			// Speed up fade in and fade out.
			romData[0x7c5e0] = 0x01; // Instead of 0x03, saving about 12 frames each fade in/out.
			romData[0x6c74a] = 0x01; // instead of 0x0f, saving 1+ second / castle door opening.
		}

		private void speedUpMusic()
		{
			// Win tune
			romData[0x6666a] = 0x01;
			romData[0x6666c] = 0x01;
			romData[0x6667d] = 0x01;

			// Level up
			romData[0x43d22] = 0x01;
			romData[0x43d27] = 0x01;
			romData[0x43d2b] = 0x01;

			// Save music
			romData[0x6677d] = 0x01;
			romData[0x6677f] = 0x01;
			romData[0x66781] = 0x01;
			romData[0x66783] = 0x01;
			romData[0x66786] = 0x01;
			romData[0x66788] = 0x01;
			romData[0x6678c] = 0x01;
			romData[0x66795] = 0x01;
			romData[0x6679b] = 0x01;

			// Revive music
			romData[0x66635] = 0x81;
			romData[0x6663a] = 0x01;
			romData[0x6663e] = 0x81;
			romData[0x66641] = 0x81;
			romData[0x66644] = 0x81;

			romData[0x6664b] = 0x81;
			romData[0x6664e] = 0x01;
			romData[0x66650] = 0x81;

			romData[0x66655] = 0x81;
			romData[0x6665b] = 0x01;

			// Recruit music
			romData[0x43c94] = 0x81;
			romData[0x43cb8] = 0x81;
			romData[0x43cc3] = 0x01;
			romData[0x43cd0] = 0x01;
			romData[0x43cd3] = 0x81;

			romData[0x43cff] = 0x01;
			romData[0x43d11] = 0x81;
			romData[0x43d15] = 0x81;
			romData[0x43d1b] = 0x01;

			// Inn
			romData[0x6661a] = 0x01;
			romData[0x66622] = 0x81;

			// Key item jingle
			romData[0x5fe91] = 0x81;
			romData[0x5fe99] = 0x81;
			romData[0x5fe9c] = 0x81;
			romData[0x5fea3] = 0x81;
		}

		private void speedyText()
		{
			romData[0x58393] = 0x60;

			byte[] speedyTextBlock = { 0xad, 0x0b, 0x05,
				0xf0, 0x03,
				0x20, 0x2d, 0xc6,
				0xad, 0x53, 0x05,
				0x30, 0x12,
				0xad, 0x52, 0x05,
				0xc9, 0x08,
				0xd0, 0x0b,
				0x20, 0xfb, 0x83,
				0xa9, 0x07,
				0x8d, 0x52, 0x05,
				0x20, 0xe7, 0x85,
				0x60 };

			for (int i = 0; i < speedyTextBlock.Length; i++)
				romData[0x583E8 + i] = speedyTextBlock[i];

			romData[0x58746] = 0xa2;
			romData[0x58747] = 0x18;

			byte[] compressionBlock = { 0xa9, 0x5c,
				0xa6, 0x5e,
				0xa8,
				0xca,
				0x10, 0x09,
				0x84, 0x5f,
				0x20, 0x7a, 0xdf,
				0xa4, 0x5f,
				0xa2, 0x17,
				0x06, 0x9a,
				0x26, 0x99,
				0x26, 0x98,
				0xb9, 0x35, 0x88,
				0x90, 0x03,
				0xb9, 0xd8, 0x87,
				0x10, 0xe3,
				0x86, 0x5e,
				0x29, 0x7f,
				0x60 };

			for (int i = 0; i < compressionBlock.Length; i++)
				romData[0x587b8 + i] = compressionBlock[i];
		}

		private void randomizeHeroStatsV2(Random r1)
		{
			int[,] min =
			{
				{ 6, 5, 12, 3, 4, 20, 5 },
				{ 3, 5, 9, 3, 2, 14, 7 },
				{ 4, 4, 9, 3, 6, 14, 7 },
				{ 3, 5, 12, 4, 6, 20, 9 },
				{ 3, 5, 7, 4, 2, 10, 10 },
				{ 4, 5, 10, 5, 3, 16, 0 },
				{ 7, 4, 13, 1, 2, 22, 0 },
				{ 12, 7, 8, 4, 1, 12, 0 }
			};

			int[,] max =
			{
				{ 200, 100, 190, 75, 110, 400, 240 },
				{ 65, 145, 155, 90, 155, 300, 195 },
				{ 95, 75, 100, 115, 140, 180, 195 },
				{ 36, 160, 140, 155, 230, 250, 315 },
				{ 32, 170, 130, 150, 220, 240, 270 },
				{ 120, 92, 155, 70, 60, 300, 0 },
				{ 245, 38, 300, 30, 45, 600, 0 },
				{ 250, 275, 220, 35, 210, 440, 0 }
			};
			// Skip the random bit
			byte[] romPlugin = { 0xa9, 0x20, 0x20, 0x91, 0xff };
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[0x49e09 + lnI] = romPlugin[lnI];

			romPlugin = new byte[] { 0x4c, 0x00, 0x90 }; // JMP 9000
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[0x81e0e + lnI] = romPlugin[lnI];

			romPlugin = new byte[] { 0xa9, 0x12, 0x20, 0x91, 0xff }; // Will need to jump back to 9DFF
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[0x81e13 + lnI] = romPlugin[lnI];

			for (int lnI = 0; lnI < 8; lnI++)
			{
				for (int lnJ = 0; lnJ < 7; lnJ++)
				{
					int byteToUse = 0x82010 + (lnI * 512) + (lnJ * 50);
					int[] statGains;
					if (oHeroStats == 4)
						statGains = inverted_power_curve(1, lnJ < 6 ? (r1.Next() % 225) + 30 : (r1.Next() % 400) + 150, 50, 1.0, r1);
					else
						statGains = inverted_power_curve(min[lnI, lnJ], max[lnI, lnJ], 50, 1.0, r1);

					if (lnJ < 6 || lnI < 5)
						for (int lnK = 0; lnK < statGains.Length - 1; lnK++)
							romData[byteToUse + lnK] = (byte)(statGains[lnK + 1] - statGains[lnK]);
				}
			}

			// And now to code all of this... starting from 0x81010 (9000)
			// 000B = Stat to use (A5 0B - LDA $000B)
			// 0079 - Character to use.  01/1F/3D/5B/79/97/B5/D3
			// Decrease 0079.  Then Store into 0004, then store 00 into 0005.  Then load #$1E into the accumulator, then perform division at C851.
			int pluginByte = 0x81010;
			romPlugin = new byte[] { 0xc6, 0x79,
									 0xa5, 0x04,
									 0xa9, 0x00,
									 0xa5, 0x05,
									 0xa9, 0x1e,
									 0x20, 0x51, 0xc8 }; // Division algorithm
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[pluginByte + lnI] = romPlugin[lnI];
			pluginByte += romPlugin.Length;

			// This gets the character number.  ASL, CLC, ADC #$A0.  Place into a variable, place 00 in the variable previous.
			romPlugin = new byte[] { 0xa5, 0x04,
									 0x0a,
									 0x18,
									 0x69, 0xa0,
									 0x8d, 0x01, 0x61 };
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[pluginByte + lnI] = romPlugin[lnI];
			pluginByte += romPlugin.Length;

			// LDA $#32, place 000B into 0004, multiply using C827.  Add 0004 to new variable, add 0005 to new variable + 1.
			romPlugin = new byte[] { 0xa5, 0x0b,
									 0x85, 0x04,
									 0xa9, 0x32,
									 0x20, 0x27, 0xc8,
									 0xa5, 0x04,
									 0x8d, 0x00, 0x61,
									 0xa5, 0x05,
									 0x18,
									 0x6d, 0x01, 0x61,
									 0x8d, 0x01, 0x61 };
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[pluginByte + lnI] = romPlugin[lnI];
			pluginByte += romPlugin.Length;

			// Last piece of puzzle... what level is the person at?  Well, load $#05 to Y, load indirect 0079+Y, and you'll have the level required!  Add to new variable, add 00 w/ carry to 0005.
			romPlugin = new byte[] { 0xa0, 0x05,
									 0xb1, 0x79,
									 0x18,
									 0x6d, 0x00, 0x61,
									 0x8d, 0x00, 0x61,
									 0xad, 0x01, 0x61,
									 0x69, 0x00,
									 0x8d, 0x01, 0x61 };
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[pluginByte + lnI] = romPlugin[lnI];
			pluginByte += romPlugin.Length;

			// Now you can finally load the number stored at A000-AFFF and you're a winner!  Store to 0004 then jump back to 9DFF when you're done.
			romPlugin = new byte[] { 0xad, 0x01, 0x61,
									 0x85, 0x05,
									 0xad, 0x00, 0x61,
									 0x85, 0x04,
									 0xa0, 0x00,
									 0xb1, 0x04,
									 0x85, 0x04,
									 0x69, 0x00,
									 0x85, 0x05,
									 0x4c, 0xff, 0x9d };
			for (int lnI = 0; lnI < romPlugin.Length; lnI++)
				romData[pluginByte + lnI] = romPlugin[lnI];
			pluginByte += romPlugin.Length;
		}

		private void randomizeHeroStats(Random r1)
		{
			if (oHeroStats == 0)
				return;

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
			// 48 bytes for strength, 48 bytes for agility, 48 bytes for vitality, 48 bytes for intelligence, 48 bytes for luck, 30 bytes for mp, in that order.  NOT in character order, statistic order!
			for (int lnJ = 0; lnJ < 6; lnJ++)
			{
				for (int lnI = 0; lnI < 8; lnI++)
				{
					// Do NOT run this randomization for the MP calculations of Taloon, Ragnar, and Alena.
					if (lnJ == 5 && lnI >= 5)
						continue;

					// Adjust base stat by an amount if Silly or Ridiculous difficulty is chosen.
					if (oHeroStats == 2 || oHeroStats == 3)
					{
						int randomDir = (r1.Next() % 3);
						int difference = heroL41Gains[lnI, lnJ] / (oHeroStats == 1 ? 4 : 2);
						if (randomDir == 0)
							heroL41Gains[lnI, lnJ] -= (r1.Next() % difference);
						if (randomDir == 1)
							heroL41Gains[lnI, lnJ] += (r1.Next() % difference);
					}
					// Ludicrous is totally random
					if (oHeroStats == 4)
					{
						if (lnJ == 2)
							heroL41Gains[lnI, lnJ] = (r1.Next() % 175 + 75);
						else if (lnJ == 0)
							heroL41Gains[lnI, lnJ] = (r1.Next() % (lnI == 0 || lnI >= 5 ? 175 : 210) + (lnI == 0 || lnI >= 5 ? 75 : 40));
						else if (lnJ == 5)
							heroL41Gains[lnI, lnJ] = (r1.Next() % 300 + 100);
						else
							heroL41Gains[lnI, lnJ] = (r1.Next() % 210 + 40);

					}

					int baseStat = 0;
					for (int lnK = 0; lnK < 4; lnK++)
					{
						if (romData[byteToUse + lnK] >= 128)
							baseStat += (lnK == 0 ? 1 : lnK == 1 ? 2 : lnK == 2 ? 4 : lnK == 3 ? 8 : 16);
					}
					// Base stat is +/- their stat / 4 or 2 for silly or ridiculous difficulty
					if (oHeroStats == 2 || oHeroStats == 3)
					{
						int randomDir = r1.Next() % 3;
						int difference = baseStat / (oHeroStats == 1 ? 2 : 1);
						if (randomDir == 0 && difference >= 2)
							baseStat -= r1.Next() % difference;
						if (randomDir == 1 && difference >= 2)
							baseStat += r1.Next() % difference;
					}
					// Ludicrous difficulty is just a random number from 0-31.
					else if (oHeroStats == 4)
						baseStat = r1.Next() % 32;

					// Five randomization of levels from 0-49
					int[] levels = { 0, 0, 0, 0 };
					for (int lnK = 0; lnK < 4; lnK++)
						levels[lnK] = (byte)(r1.Next() % 50);
					Array.Sort(levels);
					// Implement with the base stat.
					for (int lnK = 0; lnK < 4; lnK++)
					{
						if ((lnK == 0 && baseStat % 2 == 1) || (lnK == 1 && baseStat % 4 >= 2) || (lnK == 2 && baseStat % 8 >= 4) || (lnK == 3 && baseStat % 16 >= 8))
							romData[byteToUse + lnK] = (byte)(128 + levels[lnK]);
						else
							romData[byteToUse + lnK] = (byte)(levels[lnK]);
					}

					// Always make the last level 99.
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
								int multLevelToUse = (multLevel == 0 ? romData[byteToUse + multLevel] % 32 : romData[byteToUse + multLevel] % 128);
								if (lnL > multLevelToUse)
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

		private void randomizeHeroSpells(Random r1)
		{
			if (oHeroSpells == 0)
				return;

			// Used for announcements
			// 80+ = Intelligence based? - Spells are in numerical order.  (Blaze, Blazemore, Blazemost, etc.)
			// 0x41129-0x4113a - Hero battle spells (18 bytes)
			// 0x4113b-0x41146 - Cristo battle spells (12 bytes)
			// 0x41147-0x41152 - Nara battle spells (12 bytes)
			// 0x41153-0x4115e - Mara battle spells (12 bytes)
			// 0x4115f-0x4116a - Brey battle spells (12 bytes)
			// 0x4116d - Hero field spells (6 BYTES ONLY)
			// 0x41173 - Cristo field spells used
			// 0x4117b - Nara field spells used
			// 0x41183 - Mara field spells.  FFs are skipped.
			// 0x4118b - Brey field spells

			// Used for actual use
			// 0x4f37d - Hero field spells REPEATED (6 BYTES ONLY)
			// 0x4f383 - Cristo field spells used
			// 0x4f38b - Nara field spells used
			// 0x4f393 - Mara field spells.  FFs are skipped.
			// 0x4f39b - Brey field spells
			// 0x4a2a1-0x4a2a8 - Hero Gap (8 bytes)
			// 0x4a2a9-0x4a2bc - Hero spell learning (20 bytes)
			// 0x4a2bd-0x4a2c4 - Gap (8 bytes) (interference!  Skip 00, something happens at E0 though!  UGH!)
			// 0x4a2c5-0x4a2d0 - Nara spell learning (12 bytes)
			// 0x4a2d1-0x4a2d8 - Gap (8 bytes)
			// 0x4a2d9-0x4a2e5 - Cristo spell learning (12 bytes)
			// 0x4a2e6-0x4a2ed - Gap (8 bytes)
			// 0x4a2ee-0x4a2fc - Mara spell learning (14 bytes)
			// 0x4a2fd-0x4a304 - Gap (8 bytes)
			// 0x4a305-0x4a313 - Brey spell learning (14 bytes)

			// 0x4f338 - Battle spells for Hero repeated
			// 0x4f34a - Battle spells for Cristo repeated
			// 0x4f356 - Battle spells for Nara repeated
			// 0x4f362 - Battle spells for Mara repeated
			// 0x4f36e - Battle spells for Brey repeated

			// 0x40f84-8 - Number of field spells for hero, Cristo, Nara, Mara, and Brey

			//// There are 64 fight spells overall, and 24 command spells overall.  Make sure that each fight spell is in the final list, then scramble after that.  Make sure there are no more than three copies of a spell, 
			//// make sure there are no duplicates in blocks 0-15, 16-39, and 40-63.  Any command spells that duplicate the fight spells should be placed in their respective blocks.
			//int[] finalFight = new int[64];
			//int[] finalCommand = new int[24];
			//for (int i = 0; i < finalFight.Length; i++) finalFight[i] = -1;
			//for (int i = 0; i < finalCommand.Length; i++) finalCommand[i] = -1;

			int[] fightSpells = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 45, 47, 50, 52 }; // 52 (12-20-20)
			int[] commandSpells = { 53, 54, 55, 56, 57, 58 }; // 18 (6-6-6)
			int[] bothSpells = { 41, 42, 43, 44, 46, 48, 49, 51 };
			for (int lnI = 0; lnI < fightSpells.Length * 20; lnI++)
				swapArray(fightSpells, (r1.Next() % fightSpells.Length), (r1.Next() % fightSpells.Length));
			for (int lnI = 0; lnI < commandSpells.Length * 20; lnI++)
				swapArray(commandSpells, (r1.Next() % commandSpells.Length), (r1.Next() % commandSpells.Length));

			// Procedure:  Come up with x unique battle spells from 0x00 to 0x35.  If there are any spell learning bytes leftover, add from 0x36-0x3b
			int[] battleSpells = { 18, 12, 12, 12, 12 };
			int[] allSpells = { 20, 12, 12, 14, 14 };
			int[] fieldSpellLimit = { 4, 7, 7, 5, 5 };

			for (int lnI = 0; lnI < 5; lnI++)
			{
				int fieldSpells = 0;
				int extraField = 0;
				List<int> battle = new List<int>();
				List<int> field = new List<int>();

				int byteToUse = 0x41129; // battle spells
				int byteToUse2 = 0x4116b + (8 * lnI); // field spells
				for (int lnJ = 0; lnJ < 8; lnJ++) romData[byteToUse2 + lnJ] = 0xff;
				byteToUse2 += (lnI == 0 ? 2 : 0);
				int byteToUse3 = 0x4f338; // battle spells repeated
				int byteToUse4 = (lnI == 0 ? 0x4a2a9 : lnI == 1 ? 0x4a2d9 : lnI == 2 ? 0x4a2c5 : lnI == 3 ? 0x4a2ee : 0x4a305); // spell learning
				int gapByte = (lnI == 0 ? 0x4a2a1 : lnI == 1 ? 0x4a2d1 : lnI == 2 ? 0x4a2bd : lnI == 3 ? 0x4a2e6 : 0x4a2fd);
				int byteToUse5 = 0x4f37a; // field spells repeated
				for (int lnJ = 0; lnJ < 8; lnJ++) romData[byteToUse5 + lnJ] = 0xff;
				byteToUse5 += (lnI == 0 ? 2 : 0);
				for (int lnJ = lnI; lnJ > 0; lnJ--)
				{
					byteToUse += battleSpells[lnJ - 1];
					byteToUse3 += battleSpells[lnJ - 1];
				}

				if (oHeroSpells == 4)
				{
					for (int lnJ = 0; lnJ < battleSpells[lnI]; lnJ++)
					{
						bool dup = false;
						romData[byteToUse + lnJ] = (byte)(r1.Next() % 0x35);
						for (int lnK = lnJ - 1; lnK >= 0; lnK--)
						{
							if ((romData[byteToUse + lnJ] == romData[byteToUse + lnK]) ||
								(romData[byteToUse + lnJ] >= 0x29 && (romData[byteToUse + lnJ] == 0x2d || romData[byteToUse + lnJ] == 0x2f
								|| romData[byteToUse + lnJ] == 0x34 || romData[byteToUse + lnJ] == 0x39 || romData[byteToUse + lnJ] == 0x32 || fieldSpells >= fieldSpellLimit[lnI])))
							{
								dup = true;
								lnJ--;
								break;
							}
						}
						if (!dup)
						{
							if (romData[byteToUse + lnJ] >= 0x29 && romData[byteToUse + lnJ] != 0x2d && romData[byteToUse + lnJ] != 0x2f
								&& romData[byteToUse + lnJ] != 0x34 && romData[byteToUse + lnJ] != 0x32 && romData[byteToUse + lnJ] != 0x39)
							{
								romData[byteToUse2 + fieldSpells] = romData[byteToUse5 + fieldSpells] = romData[byteToUse + lnJ];
								field.Add(romData[byteToUse + lnJ]);
								fieldSpells++;
							}
							battle.Add(romData[byteToUse + lnJ]);
						}
					}

					for (int lnJ = 0; lnJ < allSpells[lnI] - battleSpells[lnI]; lnJ++)
					{
						bool dup = false;
						romData[byteToUse2 + fieldSpells] = romData[byteToUse5 + fieldSpells] = (byte)((r1.Next() % 6) + 0x35);
						if (romData[byteToUse2 + fieldSpells] == 0x39)
						{
							lnJ--;
							continue;
						}
						for (int lnK = fieldSpells - 1; lnK >= 0; lnK--)
						{
							if (romData[byteToUse2 + fieldSpells] == romData[byteToUse2 + lnK])
							{
								dup = true;
								lnJ--;
								break;
							}
						}
						if (!dup)
						{
							//romData[byteToUse2 + fieldSpells] = romData[byteToUse + lnJ];
							field.Add(romData[byteToUse2 + fieldSpells]);
							fieldSpells++;
							extraField++;
						}
					}
				} else
				{
					int battleByte = 0x41129;
					if (lnI > 0)
						battleByte += 18 + ((lnI - 1) * 12);
					for (int lnJ = 0; lnJ < (lnI == 0 ? 18 : 12); lnJ++)
						battle.Add(romData[battleByte + lnJ]);

					switch (lnI)
					{
						case 0:
							field = new List<int> { 0x33, 0x2a, 0x3b, 0x35, 0x2b, 0x30 };
							fieldSpells = 6;
							break;
						case 1:
							field = new List<int> { 0x29, 0x3a, 0x2a, 0x30, 0x2b, 0x2c, 0x31 };
							fieldSpells = 7;
							break;
						case 2:
							field = new List<int> { 0x29, 0x2e, 0x2a, 0x30, 0x2b };
							fieldSpells = 5;
							break;
						case 3:
							field = new List<int> { 0x33, 0x35, 0x37 };
							fieldSpells = 3;
							break;
						case 4:
							field = new List<int> { 0x33, 0x35, 0x38, 0x36 };
							fieldSpells = 4;
							break;
					}

					for (int lnJ = 0; lnJ < fieldSpells; lnJ++)
						romData[byteToUse2 + lnJ] = romData[byteToUse5 + lnJ] = (byte)field[lnJ];
				}

				int[] battle1 = battle.ToArray();
				int[] field1 = field.ToArray();

				int[] spellLearning = new int[allSpells[lnI]];
				if (oHeroSpells == 3 || oHeroSpells == 4)
				{
					for (int lnJ = 0; lnJ < spellLearning.Length; lnJ++)
						spellLearning[lnJ] = (r1.Next() % 40) + 1;
				} else
				{
					for (int lnJ = 0; lnJ < spellLearning.Length; lnJ++)
					{
						spellLearning[lnJ] = romData[byteToUse4 + lnJ];
						if (oHeroSpells == 1)
							spellLearning[lnJ] += r1.Next() % 5 - 2;
						else if (oHeroSpells == 2)
							spellLearning[lnJ] += r1.Next() % 11 - 5;
					}
					switch (lnI)
					{
						case 0:
							field = new List<int> { 0x33, 0x2a, 0x3b, 0x35, 0x2b, 0x30 };
							fieldSpells = 6;
							break;
						case 1:
							field = new List<int> { 0x29, 0x3a, 0x2a, 0x30, 0x2b, 0x2c, 0x31 };
							fieldSpells = 7;
							break;
						case 2:
							field = new List<int> { 0x29, 0x2e, 0x2a, 0x30, 0x2b };
							fieldSpells = 5;
							break;
						case 3:
							field = new List<int> { 0x33, 0x35, 0x37 };
							fieldSpells = 3;
							break;
						case 4:
							field = new List<int> { 0x33, 0x35, 0x38, 0x36 };
							fieldSpells = 4;
							break;
					}
				}

				Array.Sort(spellLearning);
				spellLearning[0] = 1;
				for (int lnJ = 0; lnJ < battleSpells[lnI]; lnJ++)
				{
					for (int lnK = lnJ + 1; lnK < battleSpells[lnI]; lnK++)
					{
						if (battle1[lnJ] > battle1[lnK])
						{
							int temp = spellLearning[lnJ];
							spellLearning[lnJ] = spellLearning[lnK];
							spellLearning[lnK] = temp;

							temp = battle1[lnJ];
							battle1[lnJ] = battle1[lnK];
							battle1[lnK] = temp;
						}
					}
				}

				for (int lnJ = 0; lnJ < extraField; lnJ++)
				{
					for (int lnK = lnJ + 1; lnK < extraField; lnK++)
					{
						if (field1[lnJ] > field1[lnK])
						{
							int bs = battleSpells[lnI];
							int temp = spellLearning[bs + lnJ];
							spellLearning[bs + lnJ] = spellLearning[bs + lnK];
							spellLearning[bs + lnK] = temp;

							temp = field1[lnJ];
							field1[lnJ] = field1[lnK];
							field1[lnK] = temp;
						}
					}
				}

				for (int lnJ = 0; lnJ < allSpells[lnI]; lnJ++)
					romData[byteToUse4 + lnJ] = (byte)spellLearning[lnJ];

				// Determine the gap.
				// First byte is spells 00-07, second byte is spells 08-0f, and so forth.
				// Bit 1 is spell 00, bit 2 is spell 01, and so forth.
				int[] gapData = { 0, 0, 0, 0, 0, 0, 0, 0 };
				for (int lnJ = 0; lnJ < battle1.Length; lnJ++)
				{
					int battleByte = battle1[lnJ] / 8;
					int battleBit = (int)Math.Pow(2, battle1[lnJ] % 8);
					gapData[battleByte] += battleBit;
				}
				for (int lnJ = 0; lnJ < field1.Length; lnJ++)
				{
					int battleByte = field1[lnJ] / 8;
					int battleBit = (int)Math.Pow(2, field1[lnJ] % 8);
					// Must check for battle bit already set!
					if (gapData[battleByte] % (battleBit * 2) < battleBit)
						gapData[battleByte] += battleBit;
				}
				romData[0x40f84 + lnI] = (byte)fieldSpells;
				for (int lnJ = 0; lnJ < gapData.Length; lnJ++)
					romData[gapByte + lnJ] = (byte)gapData[lnJ];
			}
		}

		private void randomizeNecrosaro(Random r1)
		{
			int[,] necroStats = new int[7, 5];
			necroStats = new int[,] { { 800, 255, 70, 210, 250 }, { 650, 255, 60, 190, 220 }, { 1023, 255, 50, 100, 180 }, { 700, 255, 60, 100, 200 }, { 800, 255, 70, 230, 220 }, { 700, 255, 80, 290, 230 }, { 1350, 255, 80, 290, 230 } };
			int[,] necroAttacks = new int[7, 6];
			necroAttacks = new int[,] { { 0x32, 0x32, 0x32, 0x32, 0x32, 0x32 }, { 0x32, 0x32, 0x32, 0x32, 0x32, 0x32 }, { 0x04, 0x42, 0x08, 0x0d, 0x1d, 0x32 }, { 0x2f, 0x3e, 0x5e, 0x32, 0x3e, 0x3d }, { 0x32, 0x32, 0x3d, 0x32, 0x3d, 0x3d }, { 0x32, 0x3d, 0x32, 0x32, 0x3d, 0x32 }, { 0x5e, 0x3e, 0x5e, 0x32, 0x41, 0x32 } };
			switch (oNecroDifficulty)
			{
				case 0: // Very Easy
					for (int i = 0; i < 7; i++)
						for (int j = 0; j < 5; j++)
							necroStats[i, j] /= 10;
					necroAttacks = new int[,] { { 0x32, 0x32, 0x32, 0x32, 0x32, 0x32 }, { 0x32, 0x32, 0x32, 0x32, 0x32, 0x32 }, { 0x00, 0x43, 0x07, 0x18, 0x1d, 0x32 }, { 0x2f, 0x3c, 0x5e, 0x32, 0x3c, 0x3f }, { 0x32, 0x32, 0x3c, 0x32, 0x3c, 0x3c }, { 0x32, 0x3c, 0x32, 0x32, 0x3c, 0x32 }, { 0x5e, 0x3c, 0x5e, 0x32, 0x3f, 0x32 } };
					break;
				case 1: // Easy
					for (int i = 0; i < 7; i++)
						for (int j = 0; j < 5; j++)
							necroStats[i, j] /= 5;
					necroAttacks = new int[,] { { 0x32, 0x32, 0x32, 0xb2, 0x32, 0x32 }, { 0x32, 0x32, 0x32, 0x32, 0x32, 0x32 }, { 0x03, 0x42, 0x08, 0x0d, 0x1d, 0x32 }, { 0x2f, 0x3c, 0x5e, 0x32, 0x3c, 0x3f }, { 0x32, 0x32, 0x3c, 0x32, 0x3c, 0x3c }, { 0x32, 0x32, 0x32, 0xb2, 0x3d, 0x32 }, { 0x5e, 0x3d, 0x3d, 0xb2, 0x40, 0x32 } };
					break;
				case 2: // Default
					break;
				case 3: // Normal
					break;
				case 4: // Hard
					for (int i = 0; i < 7; i++)
						for (int j = 0; j < 5; j++)
							necroStats[i, j] = necroStats[i, j] * 3 / 2;
					necroAttacks = new int[,] { { 0x32, 0x32, 0x32, 0xb2, 0x32, 0x32 }, { 0x32, 0x32, 0x32, 0x32, 0x32, 0x32 }, { 0x04, 0x42, 0x09, 0x8e, 0x1c, 0x32 }, { 0x2f, 0x3e, 0x5e, 0xb2, 0x3e, 0x3e }, { 0x32, 0x32, 0x3d, 0xb2, 0x3e, 0x3e }, { 0x32, 0x3e, 0x32, 0xb2, 0x3e, 0x32 }, { 0x5e, 0x3e, 0x3e, 0xb2, 0x41, 0x32 } };
					break;
				case 5: // Very Hard
					for (int i = 0; i < 7; i++)
						for (int j = 0; j < 5; j++)
							necroStats[i, j] = necroStats[i, j] * 2;
					necroAttacks = new int[,] { { 0x32, 0x32, 0xb2, 0xb2, 0x32, 0x32 }, { 0x32, 0x32, 0x32, 0xb2, 0x32, 0x32 }, { 0x05, 0x44, 0x09, 0x8e, 0x1c, 0x02 }, { 0x2f, 0x3e, 0x5e, 0xb2, 0x3e, 0x41 }, { 0x32, 0x32, 0x3e, 0xb2, 0x3e, 0x41 }, { 0x32, 0x3e, 0xb2, 0xb2, 0x3e, 0x41 }, { 0x5e, 0x41, 0xbe, 0xb2, 0x41, 0x32 } };
					break;
				case 6: // Chaos
					for (int i = 0; i < 7; i++)
					{
						necroStats[i, 0] = 1501 - inverted_power_curve(0, 2000, 1, 0.5, r1)[0];
						necroStats[i, 1] = 255 - inverted_power_curve(0, 255, 1, 0.5, r1)[0];
						necroStats[i, 2] = 255 - inverted_power_curve(0, 255, 1, 2, r1)[0];
						necroStats[i, 3] = 500 - inverted_power_curve(0, 500, 1, 0.5, r1)[0];
						necroStats[i, 4] = 500 - inverted_power_curve(0, 500, 1, 0.5, r1)[0];

						for (int lnJ = 0; lnJ < 6; lnJ++)
						{
							if (r1.Next() % 2 == 1)
								necroAttacks[i, lnJ] = (byte)(r1.Next() % 0x67);
							else
								necroAttacks[i, lnJ] = 0x32;
							if (necroAttacks[i, lnJ] == 0x15 || necroAttacks[i, lnJ] == 0x55)
								lnJ--; // redo randomization.  Transform and super slime is a bad idea to use here.
						}

						if (r1.Next() % 2 == 1)
							necroAttacks[i, 2] += 0x80;
						if (r1.Next() % 3 >= 1)
							necroAttacks[i, 3] += 0x80;
					}


					break;
			}
			if (oNecroDifficulty != 2 && oNecroDifficulty != 6)
			{
				double scale = oMonsterStats == 1 ? 1.5 : oMonsterStats == 2 ? 2.0 : oMonsterStats == 3 ? 3.0 : oMonsterStats == 4 ? 4.0 : 1.0;

				for (int i = 0; i < 7; i++)
					for (int j = 0; j < 5; j++)
					{
						int stat = necroStats[i, j];
						necroStats[i, j] = ScaleValue(stat, scale, 1.0, r1, false);
					}
			}

			if (oNecroDifficulty != 2)
			{
				for (int i = 0; i < 7; i++)
				{
					int byteToUse = (i == 0 ? 0x60056 + (0xae * 22) : 0x611f4 + (22 * (i - 1)));

					for (int j = 0; j < 6; j++)
						romData[byteToUse + 9 + j] = (byte)necroAttacks[i, j];

					for (int j = 0; j < 5; j++)
					{
						if (j == 0 || j == 3 || j == 4)
						{
							int byteToUse2 = (j == 0 ? 4 : j == 3 ? 5 : 6);
							int byteToUse3 = (j == 0 ? 15 : j == 3 ? 16 : 17);
							romData[byteToUse + byteToUse2] = (byte)(necroStats[i, j] % 256);
							romData[byteToUse + byteToUse3] = (byte)(romData[byteToUse + byteToUse3] - (romData[byteToUse + byteToUse3] % 4) + (necroStats[i, j] / 256));
							if (j == 0)
							{
								romData[byteToUse + 13] -= (byte)(romData[byteToUse + 13] >= 128 ? 128 : 0);
								romData[byteToUse + 14] -= (byte)(romData[byteToUse + 14] >= 128 ? 128 : 0);
								if (necroStats[i, 0] > 1450) { romData[byteToUse + 13] += 128; romData[byteToUse + 14] += 128; }
								else if (necroStats[i, 0] > 1300)
									romData[byteToUse + 14] += 128;
								else if (necroStats[i, 0] > 1150)
									romData[byteToUse + 13] += 128;
							}
						}
						else
						{
							int byteToUse2 = (j == 1 ? 3 : 2);
							romData[byteToUse + byteToUse2] = (byte)(necroStats[i, j]);
						}
					}
				}
			}
		}

		private void randomizeMonsterAttacks(Random r1)
		{
			if (oMonsterAttacks == 0)
				return;

			int[] level1Moves = { 0x00, 0x07, 0x16, 0x1c, 0x1e, 0x25, 0x2a, 0x2d, 0x30, 0x32, 0x32, 0x32, 0x32, 0x32, 0x36, 0x47, 0x48 };
			int[] level2Moves = { 0x00, 0x03, 0x07, 0x0a, 0x10, 0x12, 0x13, 0x16, 0x17, 0x1c, 0x1d, 0x1e, 0x22, 0x25, 0x2a, 0x2d, 0x30, 0x32, 0x32, 0x32, 0x32, 0x34, 0x35, 0x36, 0x38, 0x3c, 0x3f, 0x42, 0x43, 0x47, 0x48, 0x4b, 0x4c, 0x57, 0x58, 0x5f, 0x64 };
			int[] level3Moves = { 0x00, 0x01, 0x03, 0x04, 0x07, 0x08, 0x0a, 0x0b, 0x0d, 0x10, 0x11, 0x12, 0x13, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f, 0x25, 0x26, 0x2d, 0x2e, 0x30, 0x32, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3c, 0x3d, 0x3f, 0x40, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4d, 0x4e, 0x4f, 0x56, 0x58, 0x5d, 0x5e, 0x62, 0x63, 0x66 };
			int[] level4Moves = { 0x01, 0x02, 0x04, 0x05, 0x08, 0x09, 0x0b, 0x0c, 0x0d, 0x0e, 0x10, 0x14, 0x17, 0x1a, 0x1d, 0x1f, 0x20, 0x21, 0x23, 0x24, 0x26, 0x27, 0x28, 0x2e, 0x31, 0x32, 0x34, 0x37, 0x39, 0x3a, 0x3d, 0x3e, 0x40, 0x41, 0x42, 0x44, 0x45, 0x49, 0x4a, 0x4d, 0x50, 0x51, 0x53, 0x54, 0x56, 0x58, 0x59, 0x5a, 0x5d, 0x60, 0x62, 0x63 };
			int[] level5Moves = { 0x02, 0x05, 0x06, 0x09, 0x0c, 0x0e, 0x10, 0x17, 0x1a, 0x1d, 0x24, 0x27, 0x28, 0x29, 0x31, 0x32, 0x34, 0x37, 0x3a, 0x3e, 0x41, 0x44, 0x49, 0x4a, 0x52, 0x53, 0x56, 0x59, 0x5a, 0x60 };
			int[] earlyBossMoves1 = { 0x00, 0x03, 0x07, 0x0a, 0x17, 0x18, 0x22, 0x25, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x30,
									0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32,
									0x38, 0x3c, 0x3f, 0x47, 0x4b, 0x4c, 0x58, 0x5e, 0x5f, 0x61, 0x64 };
			int[] earlyBossMoves2 = { 0x00, 0x03, 0x07, 0x0a, 0x10, 0x16, 0x17, 0x18, 0x22, 0x25, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x30, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x35, 0x38, 0x3c, 0x3f, 0x42, 0x45, 0x47, 0x4b, 0x4c, 0x4f, 0x58, 0x5e, 0x5f, 0x61, 0x62, 0x64 };
			int[] weirdAttackMoves = { 0x30, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };

			for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
			{
				int byteToUse = 0x60056 + (monsterRank[lnI] * 22);
				int randomType = 0;
				int randomRandom = (r1.Next() % 4);
				if (oMonsterAttacks == 1)
					randomType = (randomRandom == 0 || randomRandom == 1 ? 0 : randomRandom == 2 ? 1 : 2);
				if (oMonsterAttacks == 2)
					randomType = randomRandom;
				if (oMonsterAttacks == 3)
					randomType = (randomRandom == 0 ? 1 : randomRandom == 1 ? 2 : 3);
				if (oMonsterAttacks == 4)
					randomType = 3;

				// Force early chapter bosses and Tricksy Urchin to have no worse than level 2 moves. (part 1)
				// We now have to get the lnI value, not the monster in that slot, since monsters are now shuffled FF4FE style.
				if (randomType == 3 && (lnI == 77 || lnI == 110 || lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91 || lnI == 92 || lnI == 155 || lnI == 150))
					randomType = 2;

				if (randomType == 0)
					continue;
				if (randomType == 1 && lnI != 34 && lnI != 97 && lnI != 175)
				{
					if (r1.Next() % 2 == 1)
						// weird attack pattern
						for (int lnJ = 0; lnJ < 6; lnJ++)
						{
							romData[byteToUse + 9 + lnJ] = (byte)weirdAttackMoves[r1.Next() % weirdAttackMoves.Length];
							// Chapter 1-4 Bosses and Tricksy Urchin should not critical hit or paralyze
							// We now have to get the lnI value, not the monster in that slot, since monsters are now shuffled FF4FE style.
							if ((romData[byteToUse + 9 + lnJ] == 0x33 || romData[byteToUse + 9 + lnJ] == 0x34 || romData[byteToUse + 9 + lnJ] == 0x37) &&
								(lnI == 77 || lnI == 110 || lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91 || lnI == 92 || lnI == 155 || lnI == 150))
								lnJ--;
						}
					else
						for (int lnJ = 0; lnJ < 6; lnJ++)
							romData[byteToUse + 9 + lnJ] = 0x32;
				}
				if (randomType == 2)
				{
					int moveLevel = (lnI < 38 ? 0 : lnI < 76 ? 1 : lnI < 114 ? 2 : lnI < 152 ? 3 : 4);
					// Force early chapter bosses and Tricksy Urchin to have no worse than level 2 moves. (part 2)
					if (lnI == 77 || lnI == 110 || lnI == 155)
						moveLevel = 6;
					// With no sleep attacks in Chapter 1, the C2 tournament, and Chapter 5
					if (lnI == 77 || lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91 || lnI == 92 || lnI == 150)
						moveLevel = 5;

					for (int lnJ = 0; lnJ < 6; lnJ++)
					{
						if (lnJ >= 0 && lnJ <= 2 && (lnI == 34 || lnI == 97 || lnI == 175))
							romData[byteToUse + 9 + lnJ] = 0x47;
						else
							romData[byteToUse + 9 + lnJ] = (byte)(moveLevel == 0 ? level1Moves[r1.Next() % level1Moves.Length] :
																  moveLevel == 1 ? level2Moves[r1.Next() % level2Moves.Length] :
																  moveLevel == 2 ? level3Moves[r1.Next() % level3Moves.Length] :
																  moveLevel == 3 ? level4Moves[r1.Next() % level4Moves.Length] :
																  moveLevel == 4 ? level5Moves[r1.Next() % level5Moves.Length] :
																  moveLevel == 5 ? earlyBossMoves1[r1.Next() % earlyBossMoves1.Length] :
																				   earlyBossMoves2[r1.Next() % earlyBossMoves2.Length]);

						// Linguar is not allowed to heal
						if (monsterRank[lnI] == 0xba && (romData[byteToUse + 9 + lnJ] == 0x22 || romData[byteToUse + 9 + lnJ] == 0x25 || romData[byteToUse + 9 + lnJ] == 0x5f))
							lnJ--;
					}
				}
				if (randomType == 3)
				{
					for (int lnJ = 0; lnJ < 6; lnJ++)
					{
						if (lnJ >= 0 && lnJ <= 2 && (lnI == 34 || lnI == 97 || lnI == 175))
							romData[byteToUse + 9 + lnJ] = 0x47;
						else if (r1.Next() % 2 == 1)
							romData[byteToUse + 9 + lnJ] = (byte)(r1.Next() % 0x67);
						else
							romData[byteToUse + 9 + lnJ] = 0x32;
						if (romData[byteToUse + 9 + lnJ] == 0x15 || romData[byteToUse + 9 + lnJ] == 0x55)
							lnJ--; // redo randomization.  Transform and super slime is a bad idea to use here.
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

				// If ludicrous is selected, change chances of double and triple attacks, as well as regeneration.
				// But do not allow this if it's an early chapter boss monster.
				if (oMonsterAttacks == 4 && !(lnI == 77 || lnI == 110 || lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91 || lnI == 92 || lnI == 155 || lnI == 150))
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

					// But no regeneration is allowed if a monster has less than 50 HP
					if (romData[byteToUse + 2] >= 50 || romData[byteToUse + 21] % 3 >= 1)
					{
						if (r1.Next() % 1000 <= lnI) // 100 HP / turn
						{
							romData[byteToUse + 13] += 128;
							romData[byteToUse + 14] += 128;
						}
						else if (r1.Next() % 1000 <= lnI) // 50 HP / turn
						{
							romData[byteToUse + 14] += 128;
						}
						else if (r1.Next() % 1000 <= lnI) // 25 HP / turn
						{
							romData[byteToUse + 13] += 128;
						}
					}
				}
			}
		}

		private void randomizeMonsterStats(Random r1)
		{
			if (oMonsterStats == 0)
				return;

			// If light is selected, 50% chance of maintaining same enemy pattern.  25% chance of changing it to attack only.  25% chance of the level moves above.
			// If silly is selected, 25% chance of maintaining same enemy pattern, 25% chance of attack only, 25% chance of level moves above, 25% of completely random.
			// If ridiculous is selected, 25% chance of attack only, 25% chance of level moves above, 50% chance of completely random.
			// If ludicrous is selected, 100% chance of completely random.

			for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
			{
				//// do not randomize Necrosaro.
				//if (lnI == 0xae) continue;

				int byteToUse = 0x60056 + (monsterRank[lnI] * 22);
				byte[] bosses = new byte[] { 0xb3, 0x12, 0xa4, 0xb0, 0xb1, 0xb2, 0xb4, 0xba, 0xc0, 0xbf, 0xbe, 0xb5, 0xc2, 0xc1, 0xbc, 0xb6, 0xb7, 0xb8, 0xb9, 0x91, 0x59, 0x62 };

				if (oMonsterStats <= 4)
				{
					// Do not adjust stats if a metal monster is involved...
					if (lnI != 34 && lnI != 97 && lnI != 175)
					{
						for (int lnJ = 2; lnJ <= 6; lnJ++)
						{
							// If a monster has 16 MP or less and "wins" a 50/50, then it will have anywhere between 0-16 MP.
							if (lnJ == 3)
							{
								if (romData[byteToUse + 3] <= 16 && r1.Next() % 2 == 0)
									romData[byteToUse + 3] = (byte)(r1.Next() % 16);
								//lnJ++;
							}

							double scale = oMonsterStats == 1 ? 1.5 : oMonsterStats == 2 ? 2.0 : oMonsterStats == 3 ? 3.0 : oMonsterStats == 4 ? 4.0 : 1.0;
							// Limit early bosses to half the selected stat randomization.
							if (lnI == 77 || lnI == 110 || lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91 || lnI == 92 || lnI == 155 || lnI == 150)
								scale /= 2.0;

							int stat = romData[byteToUse + lnJ] + (lnJ >= 4 ? (romData[byteToUse + lnJ + 11] % 4) * 256 : 0);
							stat = ScaleValue(stat, scale, 1.0, r1, false);

							if (lnJ == 2 && stat > 255) stat = 255;
							if (lnJ >= 4 && stat > 1020) stat = 1020;
							romData[byteToUse + lnJ] = (byte)(stat % 256);
							if (lnJ >= 4)
								romData[byteToUse + lnJ + 11] = (byte)(romData[byteToUse + lnJ + 11] - (romData[byteToUse + lnJ + 11] % 4) + (stat / 256));
						}
					}
				}
				else
				{
					for (int lnJ = 2; lnJ <= 6; lnJ++)
					{
						int stat;
						if (lnJ <= 3)
							stat = inverted_power_curve(8, 255, 1, .25, r1)[0];
						else
							stat = inverted_power_curve(8, 1020, 1, .1, r1)[0];

						if (lnJ == 2 && stat > 255) stat = 255;
						if (lnJ >= 4 && stat > 1020) stat = 1020;
						romData[byteToUse + lnJ] = (byte)(stat % 256);
						if (lnJ >= 4)
							romData[byteToUse + lnJ + 11] = (byte)(romData[byteToUse + lnJ + 11] - (romData[byteToUse + lnJ + 11] % 4) + (stat / 256));
					}
				}
			}

			if (oMonsterStats == 5)
			{
				// Rerank all monsters, then rezone them with the equivalent monster rank.
				int[] newMonsterRank = // after 0x55, 0x??????, - bisonhawk unknown
				{
							0x01, 0x00, 0x03, 0x02, 0x05, 0x08, 0x07, 0x09, 0x06, 0x0b, 0x0e, 0x0a, 0x11, 0x0d, 0x0f, 0x14, // 6 XP - 0-15
							0x0c, 0x1c, 0x1a, 0x18, 0x13, 0x10, 0x1f, 0x26, 0x16, 0x1e, 0x19, 0x17, 0x24, 0x1b, 0x22, 0x23, // 15 XP - 16-31
							0x15, 0x1d, 0x5c, 0x2a, 0x20, 0x27, 0x25, 0x21, 0x43, 0x28, 0x2f, 0x04, 0x31, 0x2c, 0x3c, 0x29, // 27 XP - 32-47
							0x3d, 0x2d, 0x36, 0x45, 0x2e, 0x38, 0x39, 0x33, 0x42, 0x3e, 0x58, 0x4d, 0x40, 0x4a, 0x32, 0x47, // 45 XP - 48-63
							0x2b, 0x35, 0x52, 0x48, 0x46, 0x37, 0x4c, 0xaf, 0x34, 0x5e, 0x3a, 0x4f, 0x66, 0xb3, 0x3b, 0x49, // 77 XP - 64-79
							0xb0, 0xb1, 0x56, 0x41, 0x51, 0x50, 0x55, 0x57, 0x44, 0x5a, 0x3f, 0xb2, 0xba, 0x30, 0x53, 0x60, // 104 XP - 80-95
							0x5b, 0x75, 0x5f, 0x4b, 0x68, 0x63, 0x5d, 0x54, 0x6b, 0x61, 0x67, 0x6e, 0x6a, 0x76, 0x12, 0x69, // 96-111
							0x70, 0x59, 0x78, 0x72, 0xab, 0x7c, 0x7d, 0x73, 0x71, 0x6c, 0x6f, 0x7f, 0x77, 0x74, 0x64, 0x86, // 112-127
							0x7a, 0x6d, 0x79, 0x7b, 0x80, 0x81, 0x7e, 0x87, 0x83, 0x88, 0x85, 0x82, 0x84, 0x62, 0x8c, 0x97, // 128-143
							0x89, 0x8a, 0x8d, 0x8b, 0x93, 0x90, 0xc0, 0x9c, 0x8e, 0x98, 0x95, 0xb4, 0x96, 0x9f, 0x9a, 0x99, // 144-159
							0x92, 0x9d, 0xa2, 0x94, 0x9e, 0x91, 0x8f, 0xa1, 0xa9, 0x9b, 0xa0, 0xa6, 0xaa, 0xac, 0x65, 0xa8, // 160-175
							0xb8, 0xa3, 0xa4, 0xa5, 0xa7, 0xbf, 0xb9, 0xbe, 0xb7, 0xb6, 0xb5, 0xc1, 0xc2, 0xbc // 15000 - bosses start at 0xb5 - 176-189
					};

				double[] monsterStats = new double[newMonsterRank.Length];
				for (int lnJ = 0; lnJ < monsterStats.Length; lnJ++)
					monsterStats[lnJ] = 0.0f;

				for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
				{
					// Do not randomize Necrosaro nor Chapter 4 Keeleon stats.
					if (monsterRank[lnI] == 0xae || monsterRank[lnI] == 0xbb) continue;

					int byteToUse = 0x60056 + (monsterRank[lnI] * 22);
					double hp = romData[byteToUse + 4] + ((romData[byteToUse + 15] % 4) * 256);
					double atk = romData[byteToUse + 5] + ((romData[byteToUse + 16] % 4) * 256);
					double defense = romData[byteToUse + 6] + ((romData[byteToUse + 17] % 4) * 256);
					// Maximum result:  14
					// Max XP needed:  about 12,000 base
					monsterStats[lnI] = (((double)romData[byteToUse + 2] / 510.0f) + ((double)romData[byteToUse + 3] / 255.0f) +
						(hp / 255.0f) + (atk / 255.0f) + (defense / 255.0f)) + 1;

					double xp = Math.Max(Math.Pow(monsterStats[lnI], 3.55), 1);
					double gp = Math.Max(Math.Pow(monsterStats[lnI], 2.55), 1);

					romData[byteToUse + 0] = (byte)((int)xp % 256);
					romData[byteToUse + 1] = (byte)((int)xp / 256);

					romData[byteToUse + 7] = (byte)((int)gp % 256);
					romData[byteToUse + 18] -= (byte)(romData[byteToUse + 18] % 4);
					romData[byteToUse + 18] += (byte)((int)gp / 256);
				}

				// Bubble sort the monster stats.  Can't just sort the array directly since two things have to trace...
				bool swapped = true;
				while (swapped)
				{
					swapped = false;
					for (int lnI = 0; lnI < monsterRank.Length - 1; lnI++)
					{
						if (monsterStats[lnI] > monsterStats[lnI + 1])
						{
							swapped = true;
							double statHold = monsterStats[lnI];
							monsterStats[lnI] = monsterStats[lnI + 1];
							monsterStats[lnI + 1] = statHold;

							int rankHold = newMonsterRank[lnI];
							newMonsterRank[lnI] = newMonsterRank[lnI + 1];
							newMonsterRank[lnI + 1] = rankHold;
						}
					}
				}

				// Weaken the lowest scored monsters in case they have a whack stat.  Reduce the weakness as we go along.
				// Do not reduce MP.
				for (int lnI = 0; lnI < 40; lnI++)
				{
					int byteToUse = 0x60056 + (newMonsterRank[lnI] * 22);
					double mp = romData[byteToUse + 3];
					double hp = romData[byteToUse + 4] + ((romData[byteToUse + 15] % 4) * 256);
					double atk = romData[byteToUse + 5] + ((romData[byteToUse + 16] % 4) * 256);
					double defense = romData[byteToUse + 6] + ((romData[byteToUse + 17] % 4) * 256);
					mp = Math.Pow(mp, 0.5 + (lnI * .0125));
					hp = Math.Pow(hp, 0.5 + (lnI * .0125)) + 8;
					atk = Math.Pow(atk, 0.5 + (lnI * .0125)) + 8;
					defense = Math.Pow(defense, 0.5 + (lnI * .0125)) + 4;

					romData[byteToUse + 3] = (byte)mp;
					romData[byteToUse + 4] = (byte)((int)hp % 256);
					romData[byteToUse + 15] -= (byte)(romData[byteToUse + 15] % 4);
					romData[byteToUse + 15] += (byte)((int)hp / 256);
					romData[byteToUse + 5] = (byte)((int)atk % 256);
					romData[byteToUse + 16] -= (byte)(romData[byteToUse + 16] % 4);
					romData[byteToUse + 16] += (byte)((int)atk / 256);
					romData[byteToUse + 6] = (byte)((int)defense % 256);
					romData[byteToUse + 17] -= (byte)(romData[byteToUse + 17] % 4);
					romData[byteToUse + 17] += (byte)((int)defense / 256);

					// Monster score:  [0-(0.5 + 1.0 + 4.0 + 4.0 + 4.0)] + 1 - AKA 1-14.5
					monsterStats[lnI] = (((double)romData[byteToUse + 2] / 510.0f) + ((double)romData[byteToUse + 3] / 255.0f) +
						(hp / 255.0f) + (atk / 255.0f) + (defense / 255.0f)) + 1;

					double xp = Math.Max(Math.Pow(monsterStats[lnI], 3.5), 1);

					romData[byteToUse + 0] = (byte)((int)xp % 256);
					romData[byteToUse + 1] = (byte)((int)xp / 256);

					double gp = Math.Max(Math.Pow(monsterStats[lnI], 2.55), 1);

					romData[byteToUse + 7] = (byte)((int)gp % 256);
					romData[byteToUse + 18] -= (byte)(romData[byteToUse + 18] % 4);
					romData[byteToUse + 18] += (byte)((int)gp / 256);
				}

				// Go through all of the monster zones and replace the old monster with the new.
				List<int> test = monsterRank.ToList();
				for (int lnI = 0; lnI <= 0x64; lnI++)
				{
					int byteToUse = 0x612ba + (lnI * 16);
					for (int lnJ = 2; lnJ <= 13; lnJ++)
					{
						// Determine the old monster rank
						int oldRank = test.IndexOf(newMonsterRank[lnI]);
						int newMonster = newMonsterRank[oldRank];
						// Don't bring in Linguar and Imposter unless it's a single monster.
						while (lnJ != 13 && (newMonster == 0xba || newMonster == 0x99))
							newMonster = newMonsterRank[oldRank + 1];
						romData[byteToUse + lnJ] = (byte)newMonster;
					}
				}

				// Go through all of the boss fights and replace the old monster with the new.
				//int[] firstMonster = { 0x62, 0x91, 0x12, 0xbb, 0xb4, 0xb3, 0x91, 0xaf, 0xb0, 0xb1, 0xb2, 0xba, 0xc0, 0xbf,
				//	0xbe, -2, 0xb5, 0xc1, 0xc2, 0x9b, 0xbc, 0xb9, 0xb8, 0xb7, 0xb6, 0xae, -2,
				//	0x59, -2, -2, -2, -2, -2, -2 };

				int[] HPLimit = { -1, -1, 80, -1, 500, 500, -1, 100, 140, 140, 140, 160, 55, 150,
					-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
					-1, -1, -1, -1, -1, -1, -1 };
				int[] AtkLimit = { -1, -1, 60, -1, 120, 80, -1, 70, 75, 55, 85, 90, 55, 130,
					-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
					-1, -1, -1, -1, -1, -1, -1 };
				int[] DefLimit = { -1, -1, 60, -1, 70, 50, -1, 100, 120, 100, 140, 150, 50, 160,
					-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
					-1, -1, -1, -1, -1, -1, -1 };
				for (int lnI = 0; lnI < 34; lnI++)
				{
					int byteToUse = 0x6235c + (8 * lnI);
					for (int lnJ = 0; lnJ < 4; lnJ++)
					{
						// Do not swap Chapter 4 Keeleon or Necrosaro or blank (0xff)
						if (romData[byteToUse + lnJ] == 0xbb || romData[byteToUse + lnJ] == 0xae || romData[byteToUse + lnJ] == 0xff)
							continue;

						if (HPLimit[lnI] != -1)
						{
							// Determine the old monster rank
							int oldRank = test.IndexOf(romData[byteToUse + lnJ]);
							int newMonster = newMonsterRank[oldRank];
							// Don't bring in Linguar and Imposter unless it's a single monster.
							while ((newMonster == 0xba || newMonster == 0x99) && (lnJ >= 1 || romData[byteToUse + 1] != 0xff || romData[byteToUse + 2] != 0xff || romData[byteToUse + 3] != 0xff))
								newMonster = newMonsterRank[oldRank + 1];
							romData[byteToUse + lnJ] = (byte)newMonster;
							if (lnJ == 0 || byteToUse == 0x6236d || byteToUse == 0x623c5)
								firstMonster[lnI] = newMonster;

							if (HPLimit[lnI] != -1)
							{
								int byteToUse2 = 0x60056 + (newMonsterRank[lnI] * 22);

								int hp = romData[byteToUse2 + 4] + (256 * (romData[byteToUse2 + 15] % 4));
								int atk = romData[byteToUse2 + 5] + (256 * (romData[byteToUse2 + 16] % 4));
								int def = romData[byteToUse2 + 6] + (256 * (romData[byteToUse2 + 17] % 4));
								if (hp > HPLimit[lnI])
								{
									romData[byteToUse2 + 4] = (byte)(HPLimit[lnI] % 256);
									romData[byteToUse2 + 15] -= (byte)(romData[byteToUse + 15] % 4);
									romData[byteToUse2 + 15] = (byte)(HPLimit[lnI] / 256);
								}
								if (atk > AtkLimit[lnI])
								{
									romData[byteToUse2 + 5] = (byte)(AtkLimit[lnI] % 256);
									romData[byteToUse2 + 16] -= (byte)(romData[byteToUse + 16] % 4);
									romData[byteToUse2 + 16] = (byte)(AtkLimit[lnI] / 256);
								}
								if (def > DefLimit[lnI])
								{
									romData[byteToUse2 + 6] = (byte)(DefLimit[lnI] % 256);
									romData[byteToUse2 + 17] -= (byte)(romData[byteToUse + 17] % 4);
									romData[byteToUse2 + 17] = (byte)(DefLimit[lnI] / 256);
								}
							}
						}
					}
				}

				monsterRank = newMonsterRank;
			}
		}

		private void randomizeMonsterDrops(Random r1)
		{
			if (oMonsterDrops != 0)
			{
				for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
				{
					// Do not change the Chapter 2 tournament bosses drops.  They should still be herbs.
					if (lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91)
						continue;

					int byteToUse = 0x60056 + (monsterRank[lnI] * 22);

					if (oMonsterDrops == 1)
					{
						if (r1.Next() % 2 == 0)
							romData[byteToUse + 8] = 0x7f;
						else
							romData[byteToUse + 8] = L1234All[r1.Next() % L1234All.Count];
					} else if (oMonsterDrops == 2)
					{
						List<byte> drops;
						if (r1.Next() % 5 != 4)
							drops = new List<byte> { 0x53, 0x54, 0x55, 0x56, 0x58, 0x5a, 0x5e, 0x74, 0x79 };
						else if (r1.Next() % 5 != 4)
							drops = new List<byte> { 0x61, 0x62, 0x63, 0x64, 0x65 };
						else
							drops = new List<byte> { 0x50, 0x57, 0x59, 0x5b, 0x5d, 0x5f, 0x60, 0x67, 0x68, 0x69, 0x6a, 0x6b };
						romData[byteToUse + 8] = drops[r1.Next() % drops.Count];
					} else if (oMonsterDrops == 3)
					{
						List<byte> drops;
						drops = new List<byte> { 0x61, 0x62, 0x63, 0x64, 0x65 };
						romData[byteToUse + 8] = drops[r1.Next() % drops.Count];
					} else if (oMonsterDrops == 4)
					{
						List<byte> drops;
						drops = new List<byte> { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
												 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,
												 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
												 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,
												 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f };
						romData[byteToUse + 8] = drops[r1.Next() % drops.Count];
					} else if (oMonsterDrops == 5)
					{
						romData[byteToUse + 8] = 0x7f;
					}
				}
			}

			if (oMonsterDrops == 5)
			{
				for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
				{
					// Do not change the Chapter 2 tournament bosses drops.  They should still be herbs.
					if (lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91)
						continue;

					int byteToUse = 0x60056 + (monsterRank[lnI] * 22);

					romData[byteToUse + 20] -= (byte)(romData[byteToUse + 20] % 8);
					romData[byteToUse + 20] += 7;
				}
			}
			if (oMonsterDropChance != 0 && oMonsterDrops != 5)
			{
				for (int lnI = 0; lnI < monsterRank.Length; lnI++) // 0xc2 is not used; Necrosaro
				{
					// Do not change the Chapter 2 tournament bosses drops.  They should still be 100% drop rate.
					if (lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91)
						continue;

					int byteToUse = 0x60056 + (monsterRank[lnI] * 22);

					if (oMonsterDropChance == 1 && romData[byteToUse + 20] % 8 != 0)
						romData[byteToUse + 20]--;
					if (oMonsterDropChance == 2 && romData[byteToUse + 20] % 8 != 7)
						romData[byteToUse + 20]++;
					if (oMonsterDropChance == 3)
					{
						romData[byteToUse + 20] -= (byte)(romData[byteToUse + 20] % 8);
						romData[byteToUse + 20] += (byte)(r1.Next() % 8);
					}
					if (oMonsterDropChance == 4)
						romData[byteToUse + 20] -= (byte)(romData[byteToUse + 20] % 8);
				}
			}
		}

		private void randomizeTreasures(Random r1)
		{
			if (oTreasures == 0)
				return;

			int[] c1p1Treasure = {
					0x7bf38, 0x7bf37, // Cave To Izmit
                    0x7bf15, 0x7bf16, 0x7bf17, 0x7bdb7, 0x7b936 }; // Old Well - Flying Shoes - 7
			int[] c1p2Treasure = { 0x7bf47, 0x7bf48, 0x7bf49, 0x7bf4a, 0x7bf4b, 0x7bf4c }; // Loch Tower - End of C1 - 6 (13)
			int[] c2p1Treasure = { 0x7bf10, 0x7bf11, 0x7bf12, 0x7bf13, 0x7bf14 }; // Cave Of Frenor - Thief's Key - 5 (18)
			int[] c2p2Treasure = { 0x7bf41, 0x7bf42, 0x7bf43, 0x7b901 }; // Birdsong Tower - Birdsong Nectar(0x7b8f4?) - 4 (22)
			int[] c3p1Treasure = { 0x7bf2a, 0x7bf2b, // Iron Safe Cave
                    0x560e8, // Foxville fox
                    0x7bf2d, 0x7bf2e, 0x7bf2f, 0x7bf30, 0x7bf31, 0x7bf32, 0x7bf33, 0x7bf34, 0x7bf35, 0x7bf36 }; // Silver Statuette Cave - Silver Statuette - 13 (35)
			int[] c4p1Treasure = { 0x7beee, // Kievs
                    0x7bf0a, 0x7bf0b, 0x7bf0c, 0x7ba05, 0x7bf0e }; // Cave West of Kievs (couple 0x7ba05 with 0x7ba0a) - 6 (41)
			int[] c4p2Treasure = { 0x7bef0, 0x7bef1, 0x7bef2 }; // Aktemto Mine - Gunpowder Jar and Sphere Of Silence - 3 (44)
			int[] c5p1Treasure = { 
                    //0x7bf2c, // Cave Of Betrayal - Can't change this due to the way the cave works
                    0x7beef }; // Desert Inn - Symbol Of Faith - 1 (45)
			int[] c5p2Treasure = {
					0x7bf4d, 0x7bf4e, 0x7bf4f, 0x7bf50, 0x7bf51, 0x7bf52, 0x7bf53, 0x7bf54, 0x7bf55 }; // Great Lighthouse - Fire Of Serenity - 9 (54)
			int[] c5p3Treasure = {
					0x7befe, 0x7beff, 0x7bf00, 0x7bf01, 0x7bf02, 0x7bf03, // Cave Of The Padequia - Padequia Seed - 6 (60)
                     };
			int[] c5p4Treasure = { 0x7bf0f }; // Cave West Of Kievs - Pre-Magic Key - 1 (61)
			int[] c5p5Treasure = { 0x7becd, 0x7bece, 0x7becf, 0x7bed0, 0x7bed1, 0x7bed2, // Burland Castle
                    0x7beca, 0x7becb, 0x7becc, // Santeem Castle
                    0x7beda, 0x7bedb, 0x7bedd, 0x7bede, // Endor
                    0x7befb, 0x7befc, 0x7befd }; // Shrine Of Breaking Waves - Pre-Magma Staff - 16 (77)
			int[] c5p6Treasure = { 0x7bee7, // Gardenbur Castle
                    0x7bf04, 0x7bf05, 0x7bf06, 0x7bf07, 0x7bf08, 0x7bf09 }; // Cave SE Of Gardenbur - Pre-Final Key - 7 (84)
			int[] c5p7Treasure = { 0x7beeb, 0x7beec, 0x7beed, // Lakanaba
                    0x7bee3, 0x7bee4, 0x7bee5, // Branca Castle
                    0x7bf56, // Konenber
                    0x7bee6, // Gardenbur Castle
                    0x7bf39, 0x7bf3a, 0x7bf3b, // Royal Crypt
                    0x7bf64, 0x7bf65, 0x7bf66, 0x7bf67, 0x7bf68 }; // Colossus - Pre-Staff Of Transform - 16 (100)
			int[] c5p8Treasure = { 0x7bed3, 0x7bed4, 0x7bed5, 0x7bed6, // Dire Palace
                    0x7bef3, 0x7bef4, 0x7bef5, 0x7bef6, 0x7bef7, 0x7bef8, 0x7bef9, 0x7befa }; // Aktemto Bonus Round - Gas Canister & Stone Of Drought - 12 (112)
			int[] c5p9Treasure = { 0x7bf44, 0x7bf45, 0x7bf46, // World Tree
                    0x7bf18, 0x7bf19, 0x7bf1a, 0x7bf1b, 0x7bf1c, // Cascade Cave
                    0x7bf61, 0x7bf63 }; // Shrine Of Horn - All Zenithian equipment - 10 (122)
			int[] c5p10Treasure = { 0x7bf3c, 0x7bf3d, 0x7bf3e, 0x7bf3f, 0x7bf40, // Zenithian Tower
                    0x7bf1d, 0x7bf1e, 0x7bf1f, 0x7bf20, 0x7bf21, 0x7bf22, 0x7bf23, 0x7bf24, 0x7bf25, 0x7bf26, 0x7bf27, 0x7bf28, 0x7bf29, // Final Cave
                    0x7bf5c, // Radimvice Area
                    0x7bf5d, 0x7bf5d, 0x7bf5d, 0x7bf60 }; // Necrosaro's Palace - Baron's Horn & End Of Game - 23 (145)
			int[] c5Deadp1 =
			{
				0x7bd1d, // Burland
                0x7bd0f, 0x7bd16, // Santeem
                0x7bdc7, // Tempe
                0x7bd86, // Lakanaba
                0x7bd7f, 0x7bd78, 0x7bdca, // Monbaraba
                0x7bd8d, // Kievs
                0x7bd71, 0x7bdc9, // Hometown
                0x7b96a, 0x7b983 // Woodman's Shack - 13 (158)
			};
			int[] c5Deadp2 =
			{
				0x7bd6a, // Izmit
                0x7bd4e, 0x7bd55, // Bazaar
                0x7bd08, // Santeem (Thief's Key)
                0x7bdc8, // Aneaux
				0x7bedc // Endor (Chapter 2/3) - 6 (164)
			};
			int[] c5Deadp3 =
			{
				0x7bdc6, // Mintos
                0x7bdcc, // Shrine East Of Mintos
                0x7bda9, // Old Man's Island House
                0x7bdcb, 0x7b94b, // Seaside Village
                0x7bd40, 0x7bd47, // Stancia Castle
                0x7bdc5, // Riverton
                0x7bda2, 0x7bd9b, // Konenber ships -> lost forevers,
                0x7bdb0, // Cave West Of Kievs, hidden basement
                0x7bd2b, // Endor (King's Drawer - Chapter 5)
                0x7bd32, 0x7bd39, // Gardenbur Castle
                0x7bd63, 0x7bd5c // Haville - 16 (180)
			};
			int[] c5Deadp4 = { 
                0x7bd24, 0x7bdc3, // Dire Palace
                0x7bdc4, // Aktemto Bonus Round
                0x7bd94, // Gottside
                0x7bd01, // Zenithian Castle
                0x7bdcd, // Gigademon Area - 6 (186)
			}; 

			List<int> allTreasureList = new List<int>();
			allTreasureList = addTreasure(allTreasureList, c1p1Treasure);
			allTreasureList = addTreasure(allTreasureList, c1p2Treasure);
			allTreasureList = addTreasure(allTreasureList, c2p1Treasure);
			allTreasureList = addTreasure(allTreasureList, c2p2Treasure);
			allTreasureList = addTreasure(allTreasureList, c3p1Treasure);
			allTreasureList = addTreasure(allTreasureList, c4p1Treasure);
			allTreasureList = addTreasure(allTreasureList, c4p2Treasure);
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
			allTreasureList = addTreasure(allTreasureList, c5Deadp1);
			allTreasureList = addTreasure(allTreasureList, c5Deadp2);
			allTreasureList = addTreasure(allTreasureList, c5Deadp3);
			allTreasureList = addTreasure(allTreasureList, c5Deadp4);

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

			int[] l1Treasure = c1p1Treasure.Concat(c2p1Treasure).Concat(c4p1Treasure).Concat(c5p1Treasure).Concat(c5Deadp1).ToArray();
			int[] l2Treasure = c1p2Treasure.Concat(c2p2Treasure).Concat(c3p1Treasure).Concat(c4p2Treasure).Concat(c5p2Treasure).Concat(c5Deadp2).ToArray();
			int[] l3Treasure = c5p3Treasure.Concat(c5p4Treasure).Concat(c5p5Treasure).Concat(c5p6Treasure).Concat(c5p7Treasure).Concat(c5p8Treasure).Concat(c5Deadp3).ToArray();
			int[] l4Treasure = c5p9Treasure.Concat(c5p10Treasure).Concat(c5Deadp4).ToArray();

			// Completely randomize treasures first.
			foreach (int treasure in allTreasureList)
			{
				if (oTreasures >= 1 && oTreasures <= 3)
				{
					if (r1.Next() % 3 == 0)
					{
						if (oTreasures == 1)
						{
							if (l1Treasure.Contains(treasure))
								romData[treasure] = (byte)L1All[r1.Next() % L1All.Count];
							else if (l2Treasure.Contains(treasure))
								romData[treasure] = (byte)L2All[r1.Next() % L2All.Count];
							else if (l3Treasure.Contains(treasure))
								romData[treasure] = (byte)L3All[r1.Next() % L3All.Count];
							else if (l4Treasure.Contains(treasure))
								romData[treasure] = (byte)L4All[r1.Next() % L4All.Count];
						}
						else if (oTreasures == 2)
						{
							if (l1Treasure.Contains(treasure))
								romData[treasure] = (byte)L12All[r1.Next() % L12All.Count];
							else if (l2Treasure.Contains(treasure))
								romData[treasure] = (byte)L12All[r1.Next() % L12All.Count];
							else if (l3Treasure.Contains(treasure))
								romData[treasure] = (byte)L3All[r1.Next() % L3All.Count];
							else if (l4Treasure.Contains(treasure))
								romData[treasure] = (byte)L4All[r1.Next() % L4All.Count];
						}
						else if (oTreasures == 3)
						{
							romData[treasure] = (byte)L1234All[r1.Next() % L1234All.Count];
						}
					}
					else if (r1.Next() % 3 == 0 && treasure >= 0x7bec9 && treasure <= 0x7bfff) // Give out gold.  You cannot give out gold in drawers, pots, or searchable spots.
					{
						if (oTreasures == 1)
						{
							if (l1Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 15 + 128);
							else if (l2Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 30 + 128);
							else if (l3Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 50 + 128);
							else if (l4Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 80 + 128);
						}
						else if (oTreasures == 2)
						{
							if (l1Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 30 + 128);
							else if (l2Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 30 + 128);
							else if (l3Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 50 + 128);
							else if (l4Treasure.Contains(treasure))
								romData[treasure] = (byte)(r1.Next() % 80 + 128);
						}
						else if (oTreasures == 3)
						{
							romData[treasure] = (byte)(r1.Next() % 80 + 128);
						}
					}
					else
						romData[treasure] = (byte)legalTreasures2[r1.Next() % legalTreasures2.Length];
				} else if (oTreasures == 4)
				{
					romData[treasure] = (byte)(r1.Next() % 5 + 0x61);
				} else if (oTreasures == 5)
				{
					if (treasure >= 0x7bec9 && treasure <= 0x7bfff)
						romData[treasure] = (byte)(r1.Next() % 80 + 128);
					else
						romData[treasure] = 0x7f;
				}
				else if (oTreasures == 6)
				{
					romData[treasure] = (byte)(r1.Next() % 80);
				} else if (oTreasures == 7)
				{
					if (treasure >= 0x7bec9 && treasure <= 0x7bfff)
						romData[treasure] = 0x80;
					else
						romData[treasure] = 0x7f;
				}

				if (treasure == 0x7b901)
					romData[0x7b8f4] = romData[treasure];
			}

			// Then assign key items, overwriting the randomized treasures.
			int[] keyItems = { 0x6c,
					0x76, 0x75,
					0x6b, 0x6d,
					0x5d, 0x70,
					0x7c, 0xe2, 0x72, 0x1e, 0x68, 0x5c, 0x7d, 0x14, 0x37, 0x44, 0x52, // e2 instead of 7b
                    0x60, 0x67, 0x6e, 0x5f, 0x6a };
			List<int> keyItemList = new List<int> { };
			addTreasure(keyItemList, keyItems);

			int[] minItemZones = { 0,
					13, 13,
					22, 22,
					35, 35,
					44, 44, 44, 44, 44, 44, 60, 60, 60, 60, 60,
					0, 0, 0, 0, 72 };
			int[] maxItemZones = { 7,
					18, 22,
					35, 35,
					44, 44,
					54, 60, 61, 77, 100, 112, 112, 122, 122, 122, 145,
					145, 145, 145, 145, 112 };

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
			}
		}

		private void randomizeStores(Random r1)
		{
			//// We first need to adjust the prices of all the items so that the casino and medal king stores sell at a proper price.
			int[] prices = { 10, 30, 100, 1500, 550, 880, 2000, 5500, 500, 200, 1250, 350, 1600, 620, 50000, 1400,
							 2500, 20000, 600, 3300, 0, 15000, 7500, 4300, 8000, 750, 10000, 20000, 30000, 4000, 0, 4000,
							 6000, 0, 4000, 3500, // SWORD END 0x23
							 10, 70, 180, 350, 1200, 1500, 2300, 110, 400, 700, 15000, 600,
							 250, 6300, 5200, 3000, 4400, 15000, 7500, 0, 9800, 6000, 870, 8800, 1000, // ARMOR END 0x3C
							 90, 180, 650, 13000, 9000, 4700, 7100, 0, 50000, // SHIELD END 0x45
						     65, 120, 1100, 3500, 280, 0, 8, 540, 15000, 50000, // HELMET END 0x4F
							 5000, 1000, 0, 8, 10, 20, 25, 10000, 30, 6000, 500, 10000, 0, 0, 150, 0,
							 0, 120, 90, 70, 250, 550, 5, 5, 0, 4000, 0, 0, 0, 0, 1000, 0,
							 0, 0, 0, 0, 10, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0 };			

			if (oBasePriceOnPower && oStorePrices != 5)
			{
				for (int lnI = 0; lnI <= 0x4f; lnI++)
				{
					if (prices[lnI] == 0) continue; // Skip priceless equipment
					prices[lnI] = (int)Math.Pow(powerLookup(lnI, false), lnI <= 0x23 ? 2.1 : lnI <= 0x3c ? 2.4 : lnI <= 45 ? 2.6 : 2.7);
					if (prices[lnI] <= 0) prices[lnI] = 1;
				}
			}

			//// Go through the prices and plug into the ROM.  If the price is 0, make sure that 0x10 is set in the first segment.
			for (int lnI = 0; lnI < prices.Length; lnI++)
			{
				if (oStorePrices >= 1 && oStorePrices <= 4)
					prices[lnI] = ScaleValue(prices[lnI], oStorePrices == 1 ? 1.5 : oStorePrices == 2 ? 2.0 : oStorePrices == 3 ? 3.0 : oStorePrices == 4 ? 4.0 : 1.0, 1.0, r1, false);
				else if (oStorePrices == 5)
					prices[lnI] = inverted_power_curve(1, 65500, 1, 0.25, r1)[0];
				else if (oStorePrices == 6)
					prices[lnI] = 0;

				int oldValue = romData[0x40cf4 + lnI];
				bool noThrow = oldValue % 32 >= 16;
				if (noThrow && prices[lnI] >= 1)
					oldValue -= 16;
				oldValue -= oldValue % 4;
				oldValue += prices[lnI] >= 10000 ? 3 : prices[lnI] >= 1000 ? 2 : prices[lnI] >= 100 ? 1 : 0;
				romData[0x40cf4 + lnI] = (byte)oldValue;

				romData[0x40df2 + lnI] = (byte)(romData[0x40df2 + lnI] > 128 ? 128 : 0);
				romData[0x40df2 + lnI] += (byte)(prices[lnI] / (prices[lnI] >= 10000 ? 1000 : prices[lnI] >= 1000 ? 100 : prices[lnI] >= 100 ? 10 : 1));
			}

			List<int> level1WeaponStores = new List<int> // 7
			{
				0x6341f, // Burland
				0x63496, // Surene
				0x63441, 0x63446, // Lakanaba, Bonmalmo
				0x63446, // Monbaraba
				0x63462 // Branca
			};
			List<int> level1ArmorStores = new List<int> // 5
			{
				0x634a1, // Burland
				0x634b1, // Surene
				0x634c9, // Lakanaba
				0x634d1, // Monbaraba
				0x634e0 // Branca
			};
			List<int> level1ItemStores = new List<int> // 6
			{
				0x63537, // Burland
				0x6345a, // Surene
				0x63569, // Lakanaba
				0x63446, // Bonmalmo
				0x63581, // Haville
				0x63590 // Branca
			};
			List<int> level1GeneralStores = new List<int> // 3
			{
				0x63541, 0x63549, // Tempe, Frenor
				0x63579 // Kievs
			};
			List<int> level1Stores = level1WeaponStores.Concat(level1ArmorStores).Concat(level1ItemStores).Concat(level1GeneralStores).ToList();

			List<int> level2WeaponStores = new List<int> // 6
			{
				0x63425, // Izmit
				0x6342c, 0x63433, 0x6343a, // Frenor, Bazaar, Endor
				0x63453, // Haville
				0x63468, 0x63470 // Aneaux, Konenber
			};
			List<int> level2ArmorStores = new List<int> // 7
			{
				0x634a9, // Izmit
				0x634b9, 0x634c1, // Bazaar, Endor
				0x634d8, // Haville
				0x634e8, 0x634f0, 0x634f8 // Bonmalmo (Chapter 5), Aneaux, Konenber
			};
			List<int> level2ItemStores = new List<int> // 6
			{
				0x6353c, // Izmit
				0x63551, 0x63558, // Bazaar, Endor
				0x63581, // Haville
				0x63596, 0x6359d // Aneaux, Konenber
			};
			List<int> level2GeneralStores = new List<int> // 2
			{
				0x63560, // Tournament
				0x635c8 // Random Traveller
			};
			List<int> level2Stores = level2WeaponStores.Concat(level2ArmorStores).Concat(level2ItemStores).Concat(level2GeneralStores).ToList();
			List<int> lowGradeStores = level1Stores.Concat(level2Stores).ToList();
			List<int> lowGradeWeapons = level1WeaponStores.Concat(level2WeaponStores).ToList();
			List<int> lowGradeArmor = level1ArmorStores.Concat(level2ArmorStores).ToList();
			List<int> lowGradeItems = level1ItemStores.Concat(level2ItemStores).ToList();
			List<int> lowGradeGeneral = level1GeneralStores.Concat(level2GeneralStores).ToList();

			List<int> level3WeaponStores = new List<int> // 5
			{
				0x6348f, 0x63483, 0x63530, 0x6347f, 0x63489
			};
			List<int> level3ArmorStores = new List<int> // 8
			{
				0x63524, 0x63507, 0x63530, 0x634ff, 0x6350e, 0x63515, 0x6351d, 0x6352c // Mintos, Rosaville, Seaside Village, Stancia, Gardenbur, Riverton, World Tree, Gottside
			};
			List<int> level3ItemStores = new List<int> // 6
			{
				0x635b3, 0x635ae, 0x635aa, 0x635ba, 0x635a4, 0x635c0 // Mintos, Soretta, Rosaville, Seaside Village, Stancia, Gottside
			};
			List<int> level3Stores = level3WeaponStores.Concat(level3ArmorStores).Concat(level3ItemStores).ToList();

			List<int> level4GeneralStores = new List<int> // 3
			{
				0x63564, 0x63588, 0x6349b // Endor (Chapter 5), Endor Casino, Small medal king
			};
			List<int> highGradeStores = level3Stores.Concat(level4GeneralStores).ToList();

			List<int> allWeaponStores = level1WeaponStores.Concat(level2WeaponStores).Concat(level3WeaponStores).ToList();
			List<int> allArmorStores = level1ArmorStores.Concat(level2ArmorStores).Concat(level3ArmorStores).ToList();
			List<int> allItemStores = level1ItemStores.Concat(level2ItemStores).Concat(level3ItemStores).ToList();
			List<int> allStores = lowGradeStores.Concat(highGradeStores).ToList();

			if (oStoreAvailability == 1)
			{
				assignItems(level1WeaponStores, L1Weapons, prices, r1);
				assignItems(level1ArmorStores, L1Armor, prices, r1);
				assignItems(level1ItemStores, L1Items, prices, r1);
				assignItems(level1GeneralStores, L1All, prices, r1);
				assignItems(level2WeaponStores, L2Weapons, prices, r1);
				assignItems(level2ArmorStores, L2Armor, prices, r1);
				assignItems(level2ItemStores, L2Items, prices, r1);
				assignItems(level2GeneralStores, L2All, prices, r1);
				assignItems(level3WeaponStores, L3Weapons, prices, r1);
				assignItems(level3ArmorStores, L3Armor, prices, r1);
				assignItems(level3ItemStores, L3Items, prices, r1);
				assignItems(level4GeneralStores, L4All, prices, r1);
			}
			else if (oStoreAvailability == 2)
			{
				assignItems(lowGradeWeapons, L12Weapons, prices, r1);
				assignItems(lowGradeArmor, L12Armor, prices, r1);
				assignItems(lowGradeItems, L12Items, prices, r1);
				assignItems(lowGradeGeneral, L12All, prices, r1);
				assignItems(level3WeaponStores, L3Weapons, prices, r1);
				assignItems(level3ArmorStores, L3Armor, prices, r1);
				assignItems(level3ItemStores, L3Items, prices, r1);
				assignItems(level4GeneralStores, L4All, prices, r1);
			}
			else if (oStoreAvailability == 3)
			{
				assignItems(allWeaponStores, AllWeapons, prices, r1);
				assignItems(allArmorStores, AllArmor, prices, r1);
				assignItems(allItemStores, AllItems, prices, r1);
				assignItems(level4GeneralStores, L4All, prices, r1);
			}
			else if (oStoreAvailability == 4)
			{
				assignItems(allStores, L1234All, prices, r1);
			}
			else if (oStoreAvailability == 5)
			{
				assignItems(allStores, AllItems, prices, r1);
			}
			else if (oStoreAvailability == 6)
			{
				assignItems(allStores, new List<byte> { 0x61, 0x62, 0x63, 0x64, 0x65 }, prices, r1);
			}
			else if (oStoreAvailability == 7)
			{
				assignItems(allStores, new List<byte> { 0x79 }, prices, r1);
			}
		}

		private void assignItems(List<int> stores, List<byte> eligibleItems, int[] prices, Random r1)
		{
			if (oStoreNoSeeds)
			{
				eligibleItems.Remove(0x61);
				eligibleItems.Remove(0x62);
				eligibleItems.Remove(0x63);
				eligibleItems.Remove(0x64);
				eligibleItems.Remove(0x65);
			}
			foreach (int byteToUse in stores)
			{
				bool lastItem = false;
				int lnJ = 0;
				int itemDups = 0;

				do
				{
					// Force Wing of Wyvern at Bonmalmo item store so you can advance Chapter 3 plot
					if (byteToUse == 0x63573 && lnJ == 0)
					{
						romData[byteToUse] = 0x56;
						lnJ++;
						continue;
					}

					if (romData[byteToUse + lnJ] >= 128)
						lastItem = true;
					romData[byteToUse + lnJ] = eligibleItems[r1.Next() % eligibleItems.Count];

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
						}
						else
						{
							romData[0x57323] = (byte)(prices[romData[byteToUse + lnJ]] / 4000);
							if (romData[0x57323] == 0x00) romData[0x57323] = 0x01;
						}
					}

					if (itemDups < 20)
					{
						bool dupFound = false;
						for (int lnK = 0; lnK < lnJ; lnK++)
						{
							if (romData[byteToUse + lnJ] == romData[byteToUse + lnK])
							{
								itemDups++;
								dupFound = true;
								if (lastItem) romData[byteToUse + lnJ] += 128; // To reset lastItem = true
								lastItem = false;
								break;
							}
						}
						if (dupFound) continue; else itemDups = 0;
					}
					else
					{
						itemDups = 0;
					}
					if (lastItem)
						romData[byteToUse + lnJ] += 128;
					lnJ++;
				} while (!lastItem);
			}
		}

		private void randomizeHeroEquipment(Random r1)
		{
			if (oEquipChances == 0 && oEquipPowers == 0) return;
			int[] minWeapon = { 255, 255, 255, 255, 255, 255, 255, 255 };
			int[] minArmor = { 255, 255, 255, 255, 255, 255, 255, 255 };
			int[] totalChances = { 35, 25, 9, 10 };
			int[,] equipChances = { { 17, 17, 7, 7 }, { 16, 11, 6, 6 }, { 17, 14, 4, 7 }, { 12, 12, 1, 5 }, { 10, 7, 3, 3 }, { 13, 9, 4, 5 }, { 19, 13, 7, 6 }, { 7, 12, 0, 5 } };
			int[] overallChance = { 48, 39, 42, 30, 23, 31, 45, 24 };

			if (oEquipChances != 0)
			{
				for (int lnI = 0; lnI < 80; lnI++)
					romData[0x40c75 + lnI] = 0;

				for (int lnI = 0; lnI < 80; lnI++)
				{
					if (lnI == 0x21) // Leave the enhanced Zenithian sword alone.
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
						else
						{
							if (oEquipChances == 1)
								equippable = r1.Next() % totalChances[lnK] < equipChances[lnJ, lnK];
							else if (oEquipChances == 2)
								equippable = r1.Next() % 79 < overallChance[lnJ];
							else if (oEquipChances == 3)
								equippable = false;
							else if (oEquipChances == 7)
								equippable = true;
							else if (oEquipChances >= 4 && oEquipChances <= 6)
								equippable = r1.Next() % 4 < oEquipChances - 3;
						}

						if (equippable)
						{
							romData[0x40c75 + lnI] += (byte)Math.Pow(2, lnJ);
							// If you can equip the weak Zenithian Sword, you can equip the strong Zenithian Sword.
							if (lnI == 0x14)
								romData[0x40c75 + 0x21] += (byte)Math.Pow(2, lnJ);
							//if (minWeapon[lnJ] == 255 && lnK == 0)
							//{
							//	minWeapon[lnJ] = lnI;
							//	romData[0x491a1 + (lnJ * 8) + 0] = (byte)(0x80 + lnI);
							//}
							//if (minArmor[lnJ] == 255 && lnK == 1)
							//{
							//	minArmor[lnJ] = lnI;
							//	romData[0x491a1 + (lnJ * 8) + 1] = (byte)(0x80 + lnI);
							//}
						}
					}
				}
			}

			for (int lnI = 0; lnI < 8; lnI++)
			{
				// Find weakest equippable weapon
				int weak1 = 0;
				int weak2 = 0;
				int weak3 = 0;
				int weakPower1 = 255;
				int weakPower2 = 255;
				int weakPower3 = 255;
				for (int lnJ = 0; lnJ < 36; lnJ++)
				{
					if ((int)romData[0x40c75 + lnJ] % Math.Pow(2, lnI + 1) >= Math.Pow(2, lnI))
					{
						if (powerLookup(lnJ, true) < weakPower1)
						{
							weak3 = weak2;
							weakPower3 = weakPower2;
							weak2 = weak1;
							weakPower2 = weakPower1;
							weak1 = lnJ;
							weakPower1 = powerLookup(lnJ, true);
						} else if (powerLookup(lnJ, true) < weakPower2)
						{
							weak3 = weak2;
							weakPower3 = weakPower2;
							weak2 = lnJ;
							weakPower2 = powerLookup(lnJ, true);
						} else if (powerLookup(lnJ, true) < weakPower3)
						{
							weak3 = lnJ;
							weakPower3 = powerLookup(lnJ, true);
						}
					}
				}

				if (lnI == 0 || lnI == 2 || lnI == 6)
					romData[0x491a1 + (lnI * 8) + 0] = (byte)(0x80 + weak3);
				else if (lnI == 1)
					romData[0x491a1 + (lnI * 8) + 0] = (byte)(0x80 + weak2);
				else if (lnI == 4)
					romData[0x491a1 + (lnI * 8) + 0] = (byte)(0x80 + weak1);

				// Repeat for armor
				weakPower1 = weakPower2 = weakPower3 = 255;
				for (int lnJ = 36; lnJ < 61; lnJ++)
				{
					if ((int)romData[0x40c75 + lnJ] % Math.Pow(2, lnI + 1) >= Math.Pow(2, lnI))
					{
						if (powerLookup(lnJ, true) < weakPower1)
						{
							weak3 = weak2;
							weakPower3 = weakPower2;
							weak2 = weak1;
							weakPower2 = weakPower1;
							weak1 = lnJ;
							weakPower1 = powerLookup(lnJ, true);
						}
						else if (powerLookup(lnJ, true) < weakPower2)
						{
							weak3 = weak2;
							weakPower3 = weakPower2;
							weak2 = lnJ;
							weakPower2 = powerLookup(lnJ, true);
						}
						else if (powerLookup(lnJ, true) < weakPower3)
						{
							weak3 = lnJ;
							weakPower3 = powerLookup(lnJ, true);
						}
					}
				}

				if (lnI == 2 || lnI == 6)
					romData[0x491a1 + (lnI * 8) + 1] = (byte)(0x80 + weak3);
				else if (lnI == 7)
					romData[0x491a1 + (lnI * 8) + 0] = (byte)(0x80 + weak3);
				else if (lnI == 1)
					romData[0x491a1 + (lnI * 8) + 1] = (byte)(0x80 + weak2);
				else if (lnI == 3)
					romData[0x491a1 + (lnI * 8) + 0] = (byte)(0x80 + weak2);
				else if (lnI == 0 || lnI == 4)
					romData[0x491a1 + (lnI * 8) + 1] = (byte)(0x80 + weak1);
				else
					romData[0x491a1 + (lnI * 8) + 0] = (byte)(0x80 + weak1);

				weakPower1 = 255;
				for (int lnJ = 70; lnJ < 80; lnJ++)
				{
					if ((int)romData[0x40c75 + lnJ] % Math.Pow(2, lnI + 1) >= Math.Pow(2, lnI))
					{
						if (powerLookup(lnJ, true) < weakPower1)
						{
							weak1 = lnJ;
							weakPower1 = powerLookup(lnJ, true);
						}
					}

				}

				if (lnI == 0)
					romData[0x491a1 + (lnI * 8) + 2] = (byte)(0x80 + weak1);

				//if (minWeapon[lnI] != 0x00 && minWeapon[lnI] != 0x01 && minWeapon[lnI] != 0x02)
				//{
				//	int rEquip = r1.Next() % 100;
				//	if (rEquip >= 55)
				//	{
				//		romData[0x491a1 + (lnI * 8) + 0] = 0x80;
				//		romData[0x40c75 + 0x00] += (byte)Math.Pow(2, lnI);
				//	}
				//	else if (rEquip >= 20)
				//	{
				//		romData[0x491a1 + (lnI * 8) + 0] = 0x81;
				//		romData[0x40c75 + 0x01] += (byte)Math.Pow(2, lnI);
				//	}
				//	else
				//	{
				//		romData[0x491a1 + (lnI * 8) + 0] = 0x82;
				//		romData[0x40c75 + 0x02] += (byte)Math.Pow(2, lnI);
				//	}
				//}
				//if (minArmor[lnI] != 0x24 && minArmor[lnI] != 0x25 && minArmor[lnI] != 0x26)
				//{
				//	int rEquip = r1.Next() % 100;
				//	if (rEquip >= 55)
				//	{
				//		romData[0x491a1 + (lnI * 8) + 1] = 0xa4;
				//		romData[0x40c75 + 0x24] += (byte)Math.Pow(2, lnI);
				//	}
				//	else if (rEquip >= 20)
				//	{
				//		romData[0x491a1 + (lnI * 8) + 1] = 0xa5;
				//		romData[0x40c75 + 0x25] += (byte)Math.Pow(2, lnI);
				//	}
				//	else
				//	{
				//		romData[0x491a1 + (lnI * 8) + 1] = 0xa6;
				//		romData[0x40c75 + 0x26] += (byte)Math.Pow(2, lnI);
				//	}
				//}
			}
		}

		private void randomizeEquipmentPowers(Random r1)
		{
			if (oEquipPowers == 0)
				return;

			for (int lnI = 0x41df0; lnI <= 0x41e3f; lnI++)
			{
				int power = romData[lnI];
				int scaleIndex = oEquipPowers;
				if (scaleIndex >= 1 && scaleIndex <= 4)
					power = ScaleValue(power, scaleIndex == 1 ? 1.5 : scaleIndex == 2 ? 2.0 : scaleIndex == 3 ? 3.0 : 4.0, 1.0, r1);
				else if (scaleIndex == 5)
					power = inverted_power_curve(0, (lnI <= 0x41e13 ? 200 : lnI <= 0x41e2c ? 100 : lnI <= 0x41e35 ? 70 : 60), 1, 0.25, r1)[0];

				romData[lnI] = (byte)power;
			}
		}

		private int powerLookup(int lnI, bool noCurse)
		{
			int[] bonus = { 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0, 5, 0, 5, 40,
							-10, 50, -20, -30, 9999, 5, 100, 40, 80, 25, 40, 40, 10, 10, 20, 0,
							40,	9999, 40, 15,
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 20, 20, 10, 9999, 20, 20, -30, 0, -15,
							0, 0, 0, 20, 20, 5, 10, 9999, 5,
							0, 0, 0, 0, 0, 9999, -40, 0, 10, 0 };
			if (noCurse && (lnI == 0x12 || lnI == 0x13 || lnI == 0x1f || lnI == 0x3a || lnI == 0x3c || lnI == 0x4c)) return 9999;
			return romData[0x41df0 + lnI] + bonus[lnI];
		}

		private void randomizeMonsterZones(Random r1)
		{
			if (oMonsterZones == 0)
				return;

			// How monster zones are formed: ($6E45-8 is the monster, $6E49-C is the number of monsters in that group)
			// Load byte 0 of zone.  Bytes 5-7 are encouter rate adjustments from 25-200% (usually 100%; 60-7F)
			// Bytes 2-4 are AND $07, so there are eight possible "zone possibilities".
			// Start off with EF1B during the day, BE3F at night, and FF3F in a cave or tower.  Load all 12 possible monsters.  If there are any FFs in the monster list, remove that associated bit if possible.  Store the final result in a variable:  $00D2/3 and $0082/3
			// Go through each bit and, if on, add up numbers from 0x6229d+(0x12 * zone possibility above)+each bit on and store to $008A
			// Load RNG ($0012), store in $0016.  Multiply by $008A, store in $008A and $008B
			// Store $008B into $00D4.  Repeat the process mentioned two lines ago, but with D2 and D3.  
			// Once D4 is beaten, record the number of bytes it took for that to happen.  
			// If it's 0-4, load monster at the byte selected, then (maybe) load another monster at a different byte, then get RNG.  
			// If it is less than or equal to zone possibility byte 14, load another monster at a different byte, then repeat ONCE (four monster groups maximum), 
			// Then if it's NOT chapter 5, end the monster formation routine.  Otherwise do some wild thing that might add monsters to a group. (9E67 -> 9EB9)
			// if it's 5, load monster at the byte selected, then get another monster (9F34) and make a second group.  There is 1 monster in group 1, and 5 monsters in group 2 in this scenario.  (9E6A -> 9F5D)
			// if it's 6-10, load monster at the byte selected, then take zone possibility bytes 16-17, do magic, limit to 0-7(R1), load from table of (0/0/0/2/3/3/4/0), store to $008A, get RNG, multiply by $008a, 
			//               then take the high byte (i.e. divide by 256), then add from $A335+R1 to get number of monsters.  This looks to be a case of one monster group only. (9E6D -> 9E7B)
			// if it'S 11, load the monster at that byte, make it one group, and you're done. (9E70)
			// if it's 12+, load a special fight. (9E19)

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
					if (oMonsterZones == 1)
					{
						if (lnJ == 0)
						{
							romData[byteToUse + lnJ] -= (byte)(romData[byteToUse + lnJ] % 32);
							romData[byteToUse + lnJ] += (byte)(r1.Next() % 32); // Scramble the encounter groups
							continue;
						}
					}
					if (oMonsterZones == 2)
					{
						if (lnJ == 0 || lnJ == 1) continue;
						romData[byteToUse + lnJ] = (byte)(monsterRank[(r1.Next() % (maxMonster[lnI] - minMonster[lnI])) + minMonster[lnI]]);
						// Redo randomization if Linguar or Imposter are in the proceedings, due to graphical glitches, except the last byte... the solo encounter.
						if (lnJ != 13 && (romData[byteToUse + lnJ] == 0xba || romData[byteToUse + lnJ] == 0x99)) lnJ--;
						if (lnJ >= 14) romData[byteToUse + lnJ] = 0xff;
					}
					if (oMonsterZones == 3)
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
					if (oMonsterZones == 4)
					{
						if (lnJ == 0)
						{
							romData[byteToUse + lnJ] -= (byte)(romData[byteToUse + lnJ] % 32);
							romData[byteToUse + lnJ] += (byte)(r1.Next() % 32); // This should ensure that we get to see nearly all of the monsters programmed.
							continue;
						}
						if (lnJ == 1) continue;
						if (lnI == 4 || lnI == 7 || lnI == 13 || lnI == 20 || lnI == 24) // Normal first zone each chapter...
							romData[byteToUse + lnJ] = (byte)monsterRank[(r1.Next() % (maxMonster[lnI] - minMonster[lnI])) + minMonster[lnI]];
						else
							romData[byteToUse + lnJ] = (byte)monsterRank[r1.Next() % 0xbd]; // Chaotic for all the others!
						// Redo randomization if Linguar or Imposter are in the proceedings, due to graphical glitches, except the last byte... the solo encounter.
						if (lnJ != 13 && (romData[byteToUse + lnJ] == 0xba || romData[byteToUse + lnJ] == 0x99)) lnJ--;
						if (lnJ >= 14) romData[byteToUse + lnJ] = 0xff;
					}
				}
			}

			// Also rearrange boss battles.
			// Mimic, Clay Doll, Chamelion Humanoid, Keeleon I, Balzack I, Saro's Shadow, Clay Doll, Hun, Roric, Vivian, Sampson, Linguar, Tricksy Urchin, Lighthouse Bengal, (14)
			// Keeleon II, Minidemon, Balzack II, Saroknight, Bakor, Rhinoking/Bengal, Esturk, Gigademon, Anderoug(3), Infernus Shadow, Radimvice, Necrosaro, Liclick, (13)
			// Man-eater chest, Rhinoband, Imposter, Leaonar, Necrodain, Minidemon, Bengal (7)
			int[] maxBossLimit = { 0xb4, 0xbd, 27, 0xbd, 69, 50, 0xbd, -1, -1, -1, -1, -1, 60, 104, // 14
                    0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xbc, -1, 0xbc, 0xbc, 0xbc, 0xbc, -1, 60, // 13 - next to last one, and five before that, was 0xbc (Esturk and Necrosaro)
                    0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4, 0xb4 }; // 7
			int[] minBossLimit = { 0, 148, 5, 148, 20, 15, 148, -1, -1, -1, -1, -1, 20, 20, // 14
                    0, 106, 0, 0, 0, 106, 106, 148, 148, 148, 148, 148, 20, // 13
                    0, 106, 106, 106, 106, 106, 0 }; // 7
			for (int lnI = 0; lnI < 34; lnI++)
			{
				if (maxBossLimit[lnI] == -1)
					 continue;
				int byteToUse = 0x6235c + (8 * lnI);

				// 10% chance to keep the battle the way it is now... if there is more than one group involved.
				if ((romData[byteToUse + 4 + 1] >= 1 && r1.Next() % 10 == 0) || oMonsterZones <= 1)
					continue;

				for (int lnJ = 0; lnJ < 4; lnJ++)
					romData[byteToUse + lnJ] = 255;

				if (oMonsterZones == 2)
				{
					for (int lnJ = 0; lnJ < 4; lnJ++)
					{
						if (lnJ == 0 && firstMonster[lnI] >= 0)
							romData[byteToUse + 0] = (byte)firstMonster[lnI];
						else if (lnJ == 0)
							romData[byteToUse + 0] = (byte)(monsterRank[r1.Next() % (maxBossLimit[lnI] - minBossLimit[lnI]) + minBossLimit[lnI]]);
						else if (romData[byteToUse + lnJ] != 0xff)
						{
							int helperRank = 0;
							// Determine monster rank of current monster
							for (int lnK = 0; lnK < monsterRank.Length; lnK++)
								if (romData[byteToUse + lnJ] == monsterRank[lnK])
								{
									helperRank = lnK;
									break;
								}
							// Make the randomization of the other monsters +/- 10%.  Do not exceed 189 (0xbd)
							int monster = romData[byteToUse + lnJ];
							int random = monster / 10;
							monster = monster - random + (r1.Next() % (random * 2));
							romData[byteToUse + lnJ] = (byte)Math.Min(monster, 189);
						}
					}
				} else if (oMonsterZones >= 3)
				{
					for (int lnJ = 4; lnJ < 8; lnJ++)
						romData[byteToUse + lnJ] = 0;

					// Limit the number of monsters to 2 in Saro's Shadow and Cave of Betrayal Part 1, 3 in Chamelion Humanoid and Balzack, and 4 in Cave of Betrayal Part 2 and Lighthouse Bengal
					int maxMonsters = (lnI == 5 || lnI == 26 ? 2 : 
									   lnI == 2 || lnI == 4 ? 3 : 
									   lnI == 12 || lnI == 13 ? 4 : 16);

					// Figure out how many groups of monsters will be involved.
					int groups = r1.Next() % 4;
					for (int lnJ = 0; lnJ < groups + 1; lnJ++)
					{
						if (lnJ == 0 && firstMonster[lnI] >= 0)
							romData[byteToUse + 0] = (byte)firstMonster[lnI];
						else
						{
							romData[byteToUse + lnJ] = (byte)(monsterRank[r1.Next() % (maxBossLimit[lnI] - minBossLimit[lnI]) + minBossLimit[lnI]]);

							// Redo randomization if Linguar or Imposter are in the proceedings due to graphical glitches.
							if (romData[byteToUse + lnJ] == 0xba || romData[byteToUse + lnJ] == 0x99) lnJ--;

							if ((lnI == 12 || lnI == 26) && oMonsterAttacks >= 3)
							{
								// Limit Cave Of Betrayal bosses to level 2 moves only.  Sleep attacks not allowed.
								int byteToUse2 = 0x60056 + (romData[byteToUse + lnJ] * 22);
								int[] earlyBossMoves = { 0x00, 0x03, 0x07, 0x0a, 0x17, 0x18, 0x22, 0x25, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x30, 
									0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32, 0x32,
									0x38, 0x3c, 0x3f, 0x47, 0x4b, 0x4c, 0x58, 0x5e, 0x5f, 0x61, 0x64 };
								for (int lnK = 0; lnK < 6; lnK++)
									romData[byteToUse2 + 9 + lnK] = (byte)(earlyBossMoves[r1.Next() % earlyBossMoves.Length]);
							}
						}
						romData[byteToUse + lnJ + 4] = (byte)(lnJ == groups ? Math.Min(maxMonsters, 8) : 1);
						maxMonsters--;
						if (maxMonsters == 0) break;
					}
				}
			}
		}

		private void randomizeMonsterResistances(Random r1)
		{
			if (oMonsterResistances == 0)
				return;
			for (int lnI = 0; lnI < 190; lnI++) // <= 0xc2
			{
				// do not randomize Necrosaro or the metal monsters.
				if (lnI == 0xae || lnI == 0x5c || lnI == 0x75 || lnI == 0xa8) continue;

				int byteToUse = 0x60056 + (lnI * 22);

				// If silly is selected, adjust enemy resistances +/- 1.
				// If ridiculous is selected, adjust enemy resistances +/- 2.
				// If ludicrous is selected, completely randomize enemy resistances.
				for (int lnJ = 0; lnJ < 5; lnJ++)
				{
					int res1 = (romData[byteToUse + 15 + lnJ] / 8) % 4;
					int res2 = romData[byteToUse + 15 + lnJ] / 64;
					// Make early bosses have no resistances if heroes are randomized or the solo hero challenge is selected.
					if ((oC14Random || oSoloHero) && (lnI == 77 || lnI == 110 || lnI == 71 || lnI == 80 || lnI == 81 || lnI == 91 || lnI == 92 || lnI == 155 || lnI == 150))
					{
						res1 = res2 = 0;
					}
					else if (oMonsterResistances == 1)
					{
						res1 += (r1.Next() % 3) - 1;
						res2 += (r1.Next() % 3) - 1;
					}
					else if (oMonsterResistances == 2)
					{
						if (r1.Next() % 3 != 0)
						{
							res1 += (r1.Next() % 5) - 2;
							res2 += (r1.Next() % 5) - 2;
						}
					}
					else if (oMonsterResistances == 3)
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

		private void saveRom(bool calcChecksum)
		{
			string finalFile = Path.Combine(Path.GetDirectoryName(txtFileName.Text), "DW4RH_" + txtSeed.Text + "_" + (chkRandomFlags.Checked ? "RANDOM" : txtFlags.Text) + ".nes");
			File.WriteAllBytes(finalFile, romData);
			lblIntensityDesc.Text = "ROM hacking complete!  (" + finalFile + ")";
			txtCompare.Text = finalFile;

			if (calcChecksum)
			{
				try
				{
					using (var md5 = SHA1.Create())
					{
						using (var stream = File.OpenRead(finalFile))
						{
							lblNewChecksum.Text = BitConverter.ToString(md5.ComputeHash(stream)).ToLower().Replace("-", "");
						}
					}
				}
				catch (Exception ex)
				{
					lblNewChecksum.Text = "????????????????????????????????????????";
				}
			}
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
			chkC14Random.Enabled = !chkSoloHero.Checked;
			determineFlags(null, null);
		}

		private void chkShop1_CheckedChanged(object sender, EventArgs e)
		{
			if (chkCh3Shop1.Checked) chkCh3Shop25K.Checked = false;
			determineFlags(null, null);
		}

		private void chkShop25K_CheckedChanged(object sender, EventArgs e)
		{
			if (chkCh3Shop25K.Checked) chkCh3Shop1.Checked = false;
			determineFlags(null, null);
		}

		private int ScaleValue(double value, double scale, double adjustment, Random r1, bool min100 = false)
		{
			double exponent = (double)r1.Next() / int.MaxValue;
			exponent = (min100 ? exponent : exponent * 2.0 - 1.0);
			var adjustedScale = 1.0 + adjustment * (scale - 1.0);

			return (int)Math.Round(Math.Pow(adjustedScale, exponent) * value, MidpointRounding.AwayFromZero);
		}

		private double ScaleValueDouble(double value, double scale, double adjustment, Random r1, bool min100 = false)
		{
			double exponent = (double)r1.Next() / int.MaxValue;
			exponent = (min100 ? exponent : exponent * 2.0 - 1.0);
			var adjustedScale = 1.0 + adjustment * (scale - 1.0);

			return Math.Pow(adjustedScale, exponent) * value;
		}

		private int[] inverted_power_curve(int min, int max, int arraySize, double powToUse, Random r1)
		{
			int range = max - min;
			double p_range = Math.Pow(range, 1 / powToUse);
			int[] points = new int[arraySize];
			for (int i = 0; i < arraySize; i++)
			{
				double section = (double)r1.Next() / int.MaxValue;
				points[i] = (int)Math.Round(max - Math.Pow(section * p_range, powToUse));
			}
			Array.Sort(points);
			return points;
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
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkCh1InstantWell, chkCh1FlyingShoes, chkCh1Moat })); // Chapter 1
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkCh2InstantWallKick, chkCh2EndorEntry, chkCh2AwardXPTournament })); // Chapter 2
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkCh3Shop1, chkCh3Shop25K, chkCh3Tunnel1, chkCh3BuildBridges, chkCh3BuildTunnel })); // Chapter 3
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkCh4GunpowderJar, chkCh4GunpowderJar, chkBasePriceOnPower })); // Chapter 4/Pricing
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkCh5BlowUpHometown, chkCh5SymbolOfFaith, chkCh5ControlAllChars, chkInstantFinalCave, chkCh5NoZGear, chkCh5AneauxNecro })); // Chapter 5/Monsters
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkSwapMonsterStats, chkSwapBossStats })); // Monsters
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkRandomizeMap, chkSmallMap, chkSoloHero, chkC14Random, chkStoreNoSeeds, chkRandomizeNPCs })); // General/Adjustments
			flags += convertIntToChar(checkboxesToNumber(new CheckBox[] { chkSpeedUpBattles, chkSpeedUpMusic, chkDoubleWalking, chkSpeedyText, chkChapter5Start, chkCh5GasCanister })); // Adjustments

			// Combo boxes time...
			flags += convertIntToChar(cboStoreAvailability.SelectedIndex + (8 * cboStorePrices.SelectedIndex));
			flags += convertIntToChar(cboHeroStats.SelectedIndex + (8 * cboHeroSpells.SelectedIndex));
			flags += convertIntToChar(cboTreasures.SelectedIndex + (8 * cboEquipChances.SelectedIndex));
			flags += convertIntToChar(cboEquipPowers.SelectedIndex + (8 * cboNPCs.SelectedIndex));
			flags += convertIntToChar(cboSoloHero.SelectedIndex + (8 * cboCriticalHitChance.SelectedIndex));
			flags += convertIntToChar(cboTaloonInsanity.SelectedIndex + (8 * cboTaloonInsanityChance.SelectedIndex));
			flags += convertIntToChar(cboMonsterStats.SelectedIndex + (8 * cboMonsterAttacks.SelectedIndex));
			flags += convertIntToChar(cboMonsterZones.SelectedIndex + (8 * cboMonsterResistances.SelectedIndex));
			flags += convertIntToChar(cboXPBoost.SelectedIndex + (8 * cboXPRandom.SelectedIndex));
			flags += convertIntToChar(cboGoldBoost.SelectedIndex + (8 * cboGoldRandom.SelectedIndex));
			flags += convertIntToChar(cboEncounterRate.SelectedIndex + (8 * cboTaloonInsanityMoves.SelectedIndex));
			flags += convertIntToChar(cboLevelCrit.SelectedIndex + (8 * cboNecroDifficulty.SelectedIndex));
			flags += convertIntToChar(cboMonsterDrops.SelectedIndex + (8 * cboMonsterDropChance.SelectedIndex));

			txtFlags.Text = flags;
		}

		private void determineChecks(object sender, EventArgs e)
		{
			if (loading && txtFlags.Text.Length < 21)
				txtFlags.Text = "0000000000008e0022300";
			else if (txtFlags.Text.Length < 21)
				return;

			loading = true;

			string flags = txtFlags.Text;

			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(0, 1))), new CheckBox[] { chkCh1InstantWell, chkCh1FlyingShoes, chkCh1Moat }); // Chapter 1
			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(1, 1))), new CheckBox[] { chkCh2InstantWallKick, chkCh2EndorEntry, chkCh2AwardXPTournament });
			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(2, 1))), new CheckBox[] { chkCh3Shop1, chkCh3Shop25K, chkCh3Tunnel1, chkCh3BuildBridges, chkCh3BuildTunnel });
			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(3, 1))), new CheckBox[] { chkCh4GunpowderJar, chkCh4GunpowderJar, chkBasePriceOnPower });
			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(4, 1))), new CheckBox[] { chkCh5BlowUpHometown, chkCh5SymbolOfFaith, chkCh5ControlAllChars, chkInstantFinalCave, chkCh5NoZGear, chkCh5AneauxNecro });
			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(5, 1))), new CheckBox[] { chkSwapMonsterStats, chkSwapBossStats });
			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(6, 1))), new CheckBox[] { chkRandomizeMap, chkSmallMap, chkSoloHero, chkC14Random, chkStoreNoSeeds, chkRandomizeNPCs });
			numberToCheckboxes(convertChartoInt(Convert.ToChar(flags.Substring(7, 1))), new CheckBox[] { chkSpeedUpBattles, chkSpeedUpMusic, chkDoubleWalking, chkSpeedyText, chkChapter5Start, chkCh5GasCanister });

			cboStoreAvailability.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(8, 1))) % 8;
			cboStorePrices.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(8, 1))) / 8;

			cboHeroStats.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(9, 1))) % 8;
			cboHeroSpells.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(9, 1))) / 8;

			cboTreasures.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(10, 1))) % 8;
			cboEquipChances.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(10, 1))) / 8;

			cboEquipPowers.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(11, 1))) % 8;
			cboNPCs.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(11, 1))) / 8;

			cboSoloHero.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(12, 1))) % 8;
			cboCriticalHitChance.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(12, 1))) / 8;

			cboTaloonInsanity.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(13, 1))) % 8; // 0x47309, 0x472A6 - Chapter #
			cboTaloonInsanityChance.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(13, 1))) / 8; // 0x472A6 - Insanity Chance.  Default:  AND $#03

			cboMonsterStats.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(14, 1))) % 8;
			cboMonsterAttacks.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(14, 1))) / 8;

			cboMonsterZones.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(15, 1))) % 8;
			cboMonsterResistances.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(15, 1))) / 8;

			cboXPBoost.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(16, 1))) % 8;
			cboXPRandom.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(16, 1))) / 8;

			cboGoldBoost.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(17, 1))) % 8;
			cboGoldRandom.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(17, 1))) / 8;

			cboEncounterRate.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(18, 1))) % 8;
			cboTaloonInsanityMoves.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(18, 1))) / 8;

			cboLevelCrit.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(19, 1))) % 8;
			cboNecroDifficulty.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(19, 1))) / 8;

			cboMonsterDrops.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(20, 1))) % 8;
			cboMonsterDropChance.SelectedIndex = convertChartoInt(Convert.ToChar(flags.Substring(20, 1))) / 8;

			loading = false;
		}

		private int checkboxesToNumber(CheckBox[] boxes)
		{
			int number = 0;
			for (int lnI = 0; lnI < Math.Min(boxes.Length, 6); lnI++)
				number += boxes[lnI].Checked ? (int)Math.Pow(2, lnI) : 0;

			return number;
		}

		private int numberToCheckboxes(int number, CheckBox[] boxes)
		{
			for (int lnI = 0; lnI < Math.Min(boxes.Length, 6); lnI++)
				boxes[lnI].Checked = number % ((int)Math.Pow(2, lnI + 1)) >= (int)Math.Pow(2, lnI);

			return number;
		}

		private string convertIntToChar(int number)
		{
			if (number >= 0 && number <= 9)
				return number.ToString();
			if (number >= 10 && number <= 35)
				return Convert.ToChar(55 + number).ToString();
			if (number >= 36 && number <= 61)
				return Convert.ToChar(61 + number).ToString();
			if (number == 62) return "!";
			if (number == 63) return "@";
			return "";
		}

		private int convertChartoInt(char character)
		{
			if (character >= Convert.ToChar("0") && character <= Convert.ToChar("9"))
				return character - 48;
			if (character >= Convert.ToChar("A") && character <= Convert.ToChar("Z"))
				return character - 55;
			if (character >= Convert.ToChar("a") && character <= Convert.ToChar("z"))
				return character - 61;
			if (character == Convert.ToChar("!")) return 62;
			if (character == Convert.ToChar("@")) return 63;
			return 0;
		}

		private void write_bytes(int offset, byte[] data)
		{
			for (int lnI = 0; lnI < data.Length; lnI++)
				romData[offset + lnI] = data[lnI];
		}

		private void fill_bytes(int offset, byte value, int count)
		{
			for (int lnI = 0; lnI < count; lnI++)
				romData[offset + lnI] = value;
		}

		private void copy_bytes(int dest, int source, int count)
		{
			for (int lnI = 0; lnI < count; lnI++)
				romData[dest + count] = romData[source + count];
		}

		private void relocate_JSRs(int old_addr, int new_addr, int start = 0, int? end = null)
		{
			if (end == null)
				end = romData.Length;

			bool match = true;
			while (match)
			{
				int jsrIndex = Array.IndexOf(romData, 0x20, start);
				if (jsrIndex == -1 || jsrIndex > end) { match = false; continue; }

				if (romData[jsrIndex + 1] == old_addr % 256 && romData[jsrIndex + 2] == old_addr / 256)
				{
					romData[jsrIndex + 1] = (byte)(new_addr % 256);
					romData[jsrIndex + 2] = (byte)(new_addr / 256);
				}
				start = jsrIndex + 3;
			}
		}

		private void tokenize_message(byte message, byte[] pair_list)
		{

		}

		private void btnCopyChecksum_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(lblNewChecksum.Text);
		}

		private bool randomizeMapv5(Random r1)
		{
			for (int lnI = 0; lnI < 256; lnI++)
				for (int lnJ = 0; lnJ < 256; lnJ++)
				{
					if (oSmallMap && (lnI >= 128 || lnJ >= 128))
					{
						map[lnI, lnJ] = 0x06;
						island[lnI, lnJ] = 200;
					}
					else
					{
						map[lnI, lnJ] = 0x00;
						island[lnI, lnJ] = -1;
					}
				}

			for (int lnI = 0; lnI < 139; lnI++)
				for (int lnJ = 0; lnJ < 158; lnJ++)
				{
					map2[lnI, lnJ] = 0x00;
					island2[lnI, lnJ] = -1;
				}

			for (int lnI = 0; lnI < 132; lnI++)
				for (int lnJ = 0; lnJ < 156; lnJ++)
					map2[lnI, lnJ] = 0x00;


			int smallIslandSize = (r1.Next() % 24000) + 17000; // (lnI == 0 ? 1500 : lnI == 1 ? 2500 : lnI == 2 ? 1500 : lnI == 3 ? 1500 : lnI == 4 ? 5000 : 5000);
			int bigIslandSize = (r1.Next() % 12000) + 20000; // (lnI == 0 ? 1500 : lnI == 1 ? 2500 : lnI == 2 ? 1500 : lnI == 3 ? 1500 : lnI == 4 ? 5000 : 5000);
			int islandSize2 = (oSmallMap ? (r1.Next() % 1000) + 2800 : (r1.Next() % 3000) + 11000); // For Tantegel
			smallIslandSize /= (oSmallMap ? 4 : 1);
			bigIslandSize /= (oSmallMap ? 4 : 1);

			// Set up three special zones.  Zone 1000 = 20 squares and has Thief key stuff.  Zone 2000 = 40 squares and has Magic Key stuff.
			// Zone 3000 = 1 square and has Baramos stuff and end of Necrogund stuff.  It will be surrounded by one tile of mountains.
			// This takes up 94 / 256 of the total squares available.

			bool zonesCreated = false;
			List<int> noradLink = new List<int>();

			while (!zonesCreated)
			{
				zone = new int[16, 16];
				if (createZone(1000, 5, false, r1) // Burland
					&& createZone(2000, 5, false, r1, 1000) // Izmit
					&& createZone(3000, 10, false, r1) // Santeem
					&& createZone(4000, 10, false, r1, 3000) // Tempe Win
					&& createZone(6000, 15, false, r1) // Lakanaba
					&& createZone(5000, 5, false, r1, 6000) // Endor
					&& createZone(7000, 5, false, r1, 5000) // Statuette Cave
					&& createZone(9000, 20, false, r1) // Branca
					&& createZone(10000, 25, false, r1, 9000) // Konenbear
					&& createZone(8000, 20, false, r1)) // Monbarara
					zonesCreated = true;
			}

			markZoneSides();
			generateZoneMap(1000, bigIslandSize * 5 / 256, r1); // Chapter 1
			generateZoneMap(2000, bigIslandSize * 5 / 256, r1); // Chapter 1 - Part 2
			generateZoneMap(3000, bigIslandSize * 10 / 256, r1); // Chapter 2
			generateZoneMap(4000, bigIslandSize * 10 / 256, r1); // Chapter 2 - Part 2
			generateZoneMap(5000, bigIslandSize * 5 / 256, r1); // Endor Zone
			generateZoneMap(6000, bigIslandSize * 15 / 256, r1); // Chapter 3
			generateZoneMap(7000, bigIslandSize * 5 / 256, r1); // Chapter 3 part 2
			generateZoneMap(8000, bigIslandSize * 20 / 256, r1); // Chapter 4
			generateZoneMap(9000, bigIslandSize * 20 / 256, r1); // Chapter 5 start
			generateZoneMap(10000, bigIslandSize * 25 / 256, r1); // Chapter 5 post Symbol
			generateZoneMap(0, smallIslandSize * 130 / 256, r1);

			smoothMap();
			//smoothMap2();

			//createBridges(r1);
			resetIslands();

			connectLikeIslands(r1);
			// Go vertically first, then go horizontally once off island 3, then vertically onto island 4
			if (!connectIslands(r1, 3, 4, true)) return false;
			if (!connectIslands(r1, 5, 6, false)) return false;
			if (!connectIslands(r1, 5, 7, false)) return false;
			if (!connectIslands(r1, 9, 10, true)) return false;

			// We should mark islands and inaccessible land...
			int lakeNumber = 256;

			int maxPlots = 0;
			int maxLake = 0;
			int lastValidIsland = -1;

			for (int lnI = 0; lnI < 256; lnI++)
				for (int lnJ = 0; lnJ < 256; lnJ++)
				{
					if (island[lnI, lnJ] == -1)
					{
						int plots = lakePlot(lakeNumber, lnI, lnJ);
						if (plots > maxPlots)
						{
							maxPlots = plots;
							maxLake = lakeNumber;
						}
						lakeNumber++;
					}
					else
					{
						lastValidIsland = island[lnI, lnJ];
					}
				}

			// Burland
			bool midenOK = false;
			int[] midenX = new int[11];
			int[] midenY = new int[11];
			while (!midenOK)
			{
				midenX[1] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[1] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[1], midenX[1], 2, 4, new int[] { maxIsland[1] }) && legalPlacement(midenY[1] + 1, midenX[1] + 1))
				{
					midenOK = true;
					map[midenY[1] + 0, midenX[1] + 1] = 0xee;
					map[midenY[1] + 0, midenX[1] + 2] = 0xef;
					map[midenY[1] + 1, midenX[1] + 1] = 0xf2;
					map[midenY[1] + 1, midenX[1] + 2] = 0xf3;

					int byteToUse = 0x3be1c + (3 * 1);
					romData[byteToUse] = (byte)(midenX[1] + 1);
					romData[byteToUse + 1] = (byte)(midenY[1] + 1);

					setReturnPlacement(midenX[1] + 1, midenY[1], 3, true, maxLake);
				}
			}

			// Izmit
			midenOK = false;
			while (!midenOK)
			{
				midenX[2] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[2] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[2], midenX[2], 1, 1, new int[] { maxIsland[2] }) && legalPlacement(midenY[2], midenX[2]))
				{
					midenOK = true;
					map[midenY[2], midenX[2]] = 0xf0;

					int byteToUse = 0x3be1c + (3 * 10);
					romData[byteToUse] = (byte)(midenX[2]);
					romData[byteToUse + 1] = (byte)(midenY[2]);

					setReturnPlacement(midenX[2], midenY[2], 19, true, maxLake);
				}
			}

			// Santeem Castle
			midenOK = false;
			while (!midenOK)
			{
				midenX[3] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[3] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[3], midenX[3], 2, 4, new int[] { maxIsland[3] }) && legalPlacement(midenY[3] + 1, midenX[3] + 1))
				{
					midenOK = true;
					map[midenY[3] + 0, midenX[3] + 1] = 0xee;
					map[midenY[3] + 0, midenX[3] + 2] = 0xef;
					map[midenY[3] + 1, midenX[3] + 1] = 0xf2;
					map[midenY[3] + 1, midenX[3] + 2] = 0xf3;

					int byteToUse = 0x3be1c + (3 * 2);
					romData[byteToUse] = (byte)(midenX[3] + 1);
					romData[byteToUse + 1] = (byte)(midenY[3] + 1);

					setReturnPlacement(midenX[3] + 1, midenY[3], 2, true, maxLake);

					romData[0x798ac] = (byte)(midenX[3] + 1);
					romData[0x798ae] = (byte)(midenY[3] + 1);
					romData[0x798ba] = (byte)(midenX[3] + 2);
					romData[0x798b8] = (byte)(midenY[3] + 1);
				}
			}

			// Frenor
			midenOK = false;
			while (!midenOK)
			{
				midenX[4] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[4] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[4], midenX[4], 1, 1, new int[] { maxIsland[4] }) && legalPlacement(midenY[4], midenX[4]))
				{
					midenOK = true;
					map[midenY[4], midenX[4]] = 0xf0;

					int byteToUse = 0x3be1c + (3 * 11);
					romData[byteToUse] = (byte)(midenX[4]);
					romData[byteToUse + 1] = (byte)(midenY[4]);

					setReturnPlacement(midenX[4], midenY[4], 16, true, maxLake);
				}
			}

			// Endor
			midenOK = false;
			while (!midenOK)
			{
				midenX[5] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[5] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[5], midenX[5], 2, 4, new int[] { maxIsland[5] }) && legalPlacement(midenY[5] + 1, midenX[5] + 1))
				{
					midenOK = true;
					map[midenY[5] + 0, midenX[5] + 1] = 0xee;
					map[midenY[5] + 0, midenX[5] + 2] = 0xef;
					map[midenY[5] + 1, midenX[5] + 1] = 0xf2;
					map[midenY[5] + 1, midenX[5] + 2] = 0xf3;

					int byteToUse = 0x3be1c + (3 * 6);
					romData[byteToUse] = (byte)(midenX[5] + 1);
					romData[byteToUse + 1] = (byte)(midenY[5] + 1);

					setReturnPlacement(midenX[5] + 1, midenY[5], 5, true, maxLake);
				}
			}

			// Lakanaba
			midenOK = false;
			while (!midenOK)
			{
				midenX[6] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[6] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[6], midenX[6], 1, 1, new int[] { maxIsland[6] }) && legalPlacement(midenY[6], midenX[6]))
				{
					midenOK = true;
					map[midenY[6], midenX[6]] = 0xf0;

					int byteToUse = 0x3be1c + (3 * 12);
					romData[byteToUse] = (byte)(midenX[6]);
					romData[byteToUse + 1] = (byte)(midenY[6]);

					setReturnPlacement(midenX[6], midenY[6], 22, true, maxLake);
				}
			}

			// Silver Statuette Cave
			midenOK = false;
			while (!midenOK)
			{
				midenX[7] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[7] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[7], midenX[7], 1, 1, new int[] { maxIsland[7] }) && legalPlacement(midenY[7], midenX[7]))
				{
					midenOK = true;
					map[midenY[7], midenX[7]] = 0xeb;

					int byteToUse = 0x3be1c + (3 * 28);
					romData[byteToUse] = (byte)(midenX[7]);
					romData[byteToUse + 1] = (byte)(midenY[7]);
				}
			}

			// Monbarara
			midenOK = false;
			while (!midenOK)
			{
				midenX[8] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[8] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[8], midenX[8], 1, 1, new int[] { maxIsland[8] }) && legalPlacement(midenY[8], midenX[8]))
				{
					midenOK = true;
					map[midenY[8], midenX[8]] = 0xf0;

					int byteToUse = 0x3be1c + (3 * 26);
					romData[byteToUse] = (byte)(midenX[8]);
					romData[byteToUse + 1] = (byte)(midenY[8]);

					setReturnPlacement(midenX[8], midenY[8], 21, true, maxLake);
				}
			}

			// Branca Castle
			midenOK = false;
			while (!midenOK)
			{
				midenX[9] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[9] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[9], midenX[9], 2, 4, new int[] { maxIsland[9] }) && legalPlacement(midenY[9] + 1, midenX[9] + 1))
				{
					midenOK = true;
					map[midenY[9] + 0, midenX[9] + 1] = 0xee;
					map[midenY[9] + 0, midenX[9] + 2] = 0xef;
					map[midenY[9] + 1, midenX[9] + 1] = 0xf2;
					map[midenY[9] + 1, midenX[9] + 2] = 0xf3;

					int byteToUse = 0x3be1c + (3 * 3);
					romData[byteToUse] = (byte)(midenX[9] + 1);
					romData[byteToUse + 1] = (byte)(midenY[9] + 1);

					setReturnPlacement(midenX[9] + 1, midenY[9], 7, true, maxLake);
				}
			}

			// Konenber
			midenOK = false;
			while (!midenOK)
			{
				midenX[10] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				midenY[10] = 6 + (r1.Next() % (oSmallMap ? 116 : 244));
				if (validPlot(midenY[10], midenX[10], 1, 1, new int[] { maxIsland[10] }) && legalPlacement(midenY[10], midenX[10]))
				{
					midenOK = true;
					map[midenY[10], midenX[10]] = 0xf0;

					int byteToUse = 0x3be1c + (3 * 21);
					romData[byteToUse] = (byte)(midenX[10]);
					romData[byteToUse + 1] = (byte)(midenY[10]);

					setReturnPlacement(midenX[10], midenY[10], 25, true, maxLake);

					romData[0x73291] = romData[0x7afc2];
					romData[0x7329b] = romData[0x7afc3];
				}
			}

			// Don't include the opening islands in future location hunting.
			for (int lnI = 1; lnI <= 10; lnI++)
				islands.Remove(maxIsland[lnI]);

			string[] locTypes = { "C", "X", "X", "X", "C", "C", "X", "C", "C", "C", // Stancia, Burland, Santeem, Branca, Bonmalmo, Dire, Endor, Keeleon, Gardenbur, Soretta Castles
                                  // (10) Izmit, Frenor, Lakanaba, Rosaville, Tempe, Surene, Foxville, Hometown, Riverton North, Riverton South, Aneaux, Konenber, Seaside Village, Aktemto, Haville, Kievs, Monbaraba, Mintos
                                  "X", "X", "X", "T", "X", "T", "T", "T", "?", "X", "T", "X", "T", "T", "T", "T", "X", "T", 
                                  // (28) Silver Statuette, Breaking Waves, Izmit North, Izmit South, Golden Bracelet, Iron Safe, Betrayal, Branca Cave West, Branca Cave East, Magic Key Cave, Padequia Cave, Cascade Cave
                                  "X", "?", "V", "V", "V", "V", "V", "V", "V", "V", "V", "V", 
                                  // (40) Medal King Castle, Shrine to Endor North, Woodman's Shack, Desert Inn, Shrine to Endor South, Shrine W of Haville, Shrine E of Mintos, Royal Crypt, Shrine NW of Riverton,
                                  "S", "S", "S", "X", "S", "S", "S", "S", "S", 
                                  // (49) Loch Tower, Colossus Shrine North, Colossus Shrine South, Lighthouse Tower, Birdsong Tower, Elfville, Old Man's Island House, Bazaar, Well, Cave SE of Gardenbur, Gottside (59)
                                  "?", "X", "X", "W", "W", "?", "?", "?", "?", "V", "?" };

			int[] locIslands = { 99, 1, 3, 9, 6, 99, 5, 8, 99, 99,
								 2, 4, 6, 99, 3, 3, 6, 9, 99, 99, 10, 10, 99, 8, 8, 8, 8, 99,
								 7, 99, 2, 1, 4, 6, 9, 7, 9, 8, 99, 99,
								 99, 4, 9, 9, 5, 8, 99, 99, 99,
								 2, 99, 99, 10, 4, 99, 99, 4, 2, 99, 0 };

			int[] landLocs = { 1, 2, 3, 4, 6, 7,
							   10, 11, 12, 14, 15, 16, 17, 20, 21, 23, 24, 25, 26,
							   28, 30, 31, 32, 33, 34, 35, 36, 37,
							   41, 42, 43, 44, 45,
							   49, 52, 53, 56, 57 };

			int[] returnLocs = { 0, 1, 2, 6, 7, 9, 10, 11, 12, 14,
								 15, 16, 17, 18, 20, 23, 25, 26, 51, 65 };

			int[] returnPoints = { 10, 3, 2, 7, 6, 4, 5, 0, 9, 8,
								 19, 16, 22, 26, 15, -1, -1, -1, -1, 12, 17, 25, -1, 11, 18, 23, 21, 14,
								 -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
								 -1, -1, -1, -1, -1, -1, -1, -1, -1,
								 -1, -1, -1, -1, -1, -1, -1, 13, -1, -1, -1 };

			for (int lnI = 0; lnI < locTypes.Length; lnI++)
			{
				int x = 300;
				int y = 300;
				if (lnI == 1) { x = midenX[1]; y = midenY[1]; } // Burland
				else if (lnI == 2) { x = midenX[3]; y = midenY[3]; } // Santeem
				else if (lnI == 3) { x = midenX[9]; y = midenY[9]; } // Branca
				else if (lnI == 6) { x = midenX[5]; y = midenY[5]; } // Endor
				else if (lnI == 10) { x = midenX[2]; y = midenY[2]; } // Izmit
				else if (lnI == 11) { x = midenX[4]; y = midenY[4]; } // Frenor
				else if (lnI == 12) { x = midenX[6]; y = midenY[6]; } // Lakanaba
				else if (lnI == 21) { x = midenX[10]; y = midenY[10]; } // Konenbur
				else if (lnI == 26) { x = midenX[8]; y = midenY[8]; } // Monbaraba
				else if (lnI == 28) { x = midenX[7]; y = midenY[7]; } // Silver Statuette Cave
				else if (locIslands[lnI] == -100)
				{
					continue;
				}
				else
				{
					// Subtract 6 for room
					x = 6 + r1.Next() % (oSmallMap ? 128 - 6 - 6 : 256 - 6 - 6);
					y = 6 + r1.Next() % (oSmallMap ? 128 - 6 - 6 : 256 - 6 - 6);
				}

				if (lnI == 30) lnI = lnI;
				// TODO:  Ship return points, human return points, bird return points
				// If branches on locTypes, possibly a case.
				switch (locTypes[lnI])
				{
					case "C":
						if (validPlot(y - 1, x - 1, 2, 4, locIslands[lnI] <= 10 ? new int[] { maxIsland[locIslands[lnI]] } : islands.ToArray()) && legalPlacement(y + 1, x + 1) && reachable(y, x, !landLocs.Contains(lnI),
							locIslands[lnI] <= 10 ? midenX[locIslands[lnI]] : midenX[10], locIslands[lnI] <= 10 ? midenY[locIslands[lnI]] : midenY[10], maxLake, false))
						{
							map[y + 0, x + 1] = 0xee;
							map[y + 0, x + 2] = 0xef;
							map[y + 1, x + 1] = 0xf2;
							map[y + 1, x + 2] = 0xf3;

							int byteToUse = 0x3be1c + (3 * lnI);
							romData[byteToUse] = (byte)(x + 1);
							romData[byteToUse + 1] = (byte)(y + 1);

							if (returnPoints[lnI] >= 0)
								setReturnPlacement(x + 1, y + 1, returnPoints[lnI], true, maxLake);

							if (lnI == 2)
							{
								romData[0x798ac] = (byte)(x + 1);
								romData[0x798ae] = (byte)(y + 1);
								romData[0x798ba] = (byte)(x + 2);
								romData[0x798b8] = (byte)(y + 1);
							}
						}
						else
							lnI--;

						break;
					case "T": // Town
						if (validPlot(y - 1, x - 1, 3, 3, locIslands[lnI] <= 10 ? new int[] { maxIsland[locIslands[lnI]] } : islands.ToArray()) && legalPlacement(y, x) && reachable(y, x, !landLocs.Contains(lnI),
							locIslands[lnI] <= 10 ? midenX[locIslands[lnI]] : midenX[10], locIslands[lnI] <= 10 ? midenY[locIslands[lnI]] : midenY[10], maxLake, false))
						{
							map[y, x] = 0xf0;

							int byteToUse = 0x3be1c + (3 * lnI);
							romData[byteToUse] = (byte)x;
							romData[byteToUse + 1] = (byte)y;

							if (returnPoints[lnI] >= 0)
								setReturnPlacement(x, y, returnPoints[lnI], true, maxLake);

							if (lnI == 21)
							{
								romData[0x73291] = romData[0x7afc2];
								romData[0x7329b] = romData[0x7afc3];
							}
						}
						else
							lnI--;

						break;
					case "S": // Shrine
						if (validPlot(y - 1, x - 1, 3, 3, locIslands[lnI] <= 10 ? new int[] { maxIsland[locIslands[lnI]] } : islands.ToArray()) && legalPlacement(y, x) && reachable(y, x, !landLocs.Contains(lnI),
							locIslands[lnI] <= 10 ? midenX[locIslands[lnI]] : midenX[10], locIslands[lnI] <= 10 ? midenY[locIslands[lnI]] : midenY[10], maxLake, false))
						{
							map[y, x] = 0xec;

							int byteToUse = 0x3be1c + (3 * lnI);
							romData[byteToUse] = (byte)x;
							romData[byteToUse + 1] = (byte)y;
						}
						else
							lnI--;

						break;
					case "V": // Cave
						if (validPlot(y - 1, x - 1, 3, 3, locIslands[lnI] <= 10 ? new int[] { maxIsland[locIslands[lnI]] } : islands.ToArray()) && legalPlacement(y, x) && reachable(y, x, !landLocs.Contains(lnI),
							locIslands[lnI] <= 10 ? midenX[locIslands[lnI]] : midenX[10], locIslands[lnI] <= 10 ? midenY[locIslands[lnI]] : midenY[10], maxLake, false) && (lnI != 58 || towerCheck(y, x)))
						{
							map[y, x] = 0xeb;

							int byteToUse = 0x3be1c + (3 * lnI);
							romData[byteToUse] = (byte)(x);
							romData[byteToUse + 1] = (byte)(y);

							if (lnI == 31)
							{
								romData[0x22fde] = (byte)x;
								romData[0x22fe2] = (byte)y;
								romData[0x2330f] = (byte)x;
								romData[0x23311] = (byte)y;
							}
							else if (lnI == 36)
							{
								romData[0x22fb4] = (byte)x;
								romData[0x22fb8] = (byte)y;
								romData[0x23321] = (byte)x;
								romData[0x23323] = (byte)y;
							}
						}
						else
							lnI--;

						break;
					case "?":
						if (lnI == 18) // Riverton
						{
							bool baramosLegal = true;
							for (int lnJ = x - 3; lnJ <= x + 3; lnJ++)
								for (int lnK = y - 4; lnK <= y + 4; lnK++)
								{
									if (island[lnK, lnJ] != maxLake || map[lnK, lnJ] != 0x00)
										baramosLegal = false;
								}

							if (baramosLegal && legalPlacement(y - 1, x) && legalPlacement(y - 3, x) && legalPlacement(y + 2, x) && legalPlacement(y + 3, x))
							{
								for (int lnJ = -3; lnJ <= 3; lnJ++)
									for (int lnK = -2; lnK <= 2; lnK++)
										island[y + lnJ, x + lnK] = 11001;

								for (int lnJ = -3; lnJ <= 3; lnJ++)
									for (int lnK = -2; lnK <= 2; lnK++)
										map[y + lnJ, x + lnK] = 0x02;

								for (int lnJ = -3; lnJ <= 1; lnJ++)
									map[y + lnJ, x] = 0x00;

								int byteToUse = 0x3be1c + (3 * 18);
								romData[byteToUse] = (byte)(x);
								romData[byteToUse + 1] = (byte)(y - 3);

								byteToUse = 0x3be1c + (3 * 19);
								romData[byteToUse] = (byte)(x);
								romData[byteToUse + 1] = (byte)(y - 1);

								romData[0x23e8f] = romData[0x3be52];
								romData[0x23e90] = romData[0x3be53];

								romData[0x23e92] = romData[0x3be55];
								romData[0x23e93] = romData[0x3be56];

								romData[0x230e9] = (byte)(romData[0x3be53] - 1);
								romData[0x230f7] = (byte)(romData[0x3be56] + 1);

								int byteToUseReturn = 0x7af10 + (7 * 12);
								romData[byteToUseReturn + 1] = (byte)(x - 1);
								romData[byteToUseReturn + 2] = (byte)(y - 2);

								shipPlacement(byteToUseReturn + 3, y - 3, x - 1, maxLake);

								romData[byteToUseReturn + 5] = romData[0x73916] = (byte)(x - 1);
								romData[byteToUseReturn + 6] = romData[0x7391b] = (byte)(y - 1);

								// Colossus Shrine Placement
								map[y + 2, x] = 0xec;
								byteToUse = 0x3be1c + (3 * 50);
								romData[byteToUse] = (byte)x;
								romData[byteToUse + 1] = (byte)(y + 2);

								map[y + 3, x] = 0xec;
								byteToUse = 0x3be1c + (3 * 51);
								romData[byteToUse] = (byte)x;
								romData[byteToUse + 1] = (byte)(y + 3);

								romData[0x4b760] = (byte)x;
								romData[0x4b764] = (byte)(y + 2);
								romData[0x23117] = (byte)(y + 2);
								romData[0x23033] = romData[0x23121] = (byte)(y + 2);


							}
							else
								lnI--;

						}
						else if (lnI == 29) // Cave of Breaking Waves
						{
							bool baramosLegal = true;
							for (int lnJ = x - 3; lnJ <= x + 3; lnJ++)
								for (int lnK = y - 3; lnK <= y + 3; lnK++)
								{
									if (island[lnK, lnJ] != maxLake || map[lnK, lnJ] != 0x00)
										baramosLegal = false;
								}

							if (baramosLegal && legalPlacement(y + 1, x))
							{
								for (int lnJ = -1; lnJ <= 1; lnJ++)
									for (int lnK = -1; lnK <= 1; lnK++)
										map[y + lnJ, x + lnK] = 0x06;

								map[y, x] = 0xeb;
								map[y + 1, x] = 0x00;

								int byteToUse = 0x3be1c + (3 * 29);
								romData[byteToUse] = (byte)(x);
								romData[byteToUse + 1] = (byte)(y + 1);

								romData[0x23e95] = romData[0x3be73];
								romData[0x23e96] = romData[0x3be74];

								romData[0x7afff] = romData[0x3be73];
								romData[0x7b006] = (byte)(romData[0x3be74] + 1);
							}
							else
								lnI--;
						}
						else if (lnI == 55) // Old Man's Island House
						{
							bool baramosLegal = true;
							for (int lnJ = x - 2; lnJ <= x + 2; lnJ++)
								for (int lnK = y - 2; lnK <= y + 2; lnK++)
								{
									if (island[lnK, lnJ] != maxLake || map[lnK, lnJ] != 0x00)
										baramosLegal = false;
								}

							if (baramosLegal && towerCheck(y, x) && legalPlacement(y, x))
							{
								for (int lnJ = -1; lnJ <= 1; lnJ++)
									for (int lnK = -1; lnK <= 1; lnK++)
										map[y + lnJ, x + lnK] = 0x04;

								map[y, x] = 0x03;

								int byteToUse = 0x3be1c + (3 * lnI);
								romData[byteToUse] = (byte)(x);
								romData[byteToUse + 1] = (byte)(y);

								romData[0x23e89] = (byte)(x);
								romData[0x23e8a] = (byte)(y);
							}
							else
								lnI--;
						}
						else if (lnI == 56) // Bazaar
						{
							if (validPlot(y, x, 3, 3, new int[] { maxIsland[locIslands[lnI]] }) && legalPlacement(y + 1, x + 1) && reachable(y, x, false, midenX[4], midenY[4], maxLake, false) && towerCheck(y, x))
							{
								for (int lnJ = 0; lnJ <= 2; lnJ++)
									for (int lnK = 0; lnK <= 2; lnK++)
										map[y + lnJ, x + lnK] = 0x05;

								map[y + 1, x + 1] = 0x03;

								int byteToUse = 0x3be1c + (3 * lnI);
								romData[byteToUse] = (byte)(x + 1);
								romData[byteToUse + 1] = (byte)(y + 1);

								romData[0x23e8c] = (byte)(x + 1);
								romData[0x23e8d] = (byte)(y + 1);

								setReturnPlacement(x + 1, y + 1, returnPoints[lnI], true, maxLake);
							}
							else
								lnI--;
						}
						else if (lnI == 57) // Well
						{
							if (validPlot(y, x, 1, 1, new int[] { maxIsland[locIslands[lnI]] }) && legalPlacement(y, x) && reachable(y, x, false, midenX[2], midenY[2], maxLake, false) && y >= 8 && x >= 8 && map[y - 4, x - 4] < 0xe8 && towerCheck(y, x))
							{
								// Make sure the sign doesn't block a path...
								for (int signX = x - 5; signX <= x - 3; signX++)
									for (int signY = y - 5; signY <= y - 3; signY++)
									{
										if (island[signY, signX] == maxIsland[1] || island[signY, signX] == maxIsland[2] && map[signY, signX] >= 0x01 && map[signY, signX] <= 0x05)
										{
											for (int signX1 = x - 5; signX1 <= x - 3; signX1++)
												for (int signY1 = y - 5; signY1 <= y - 3; signY1++)
												{
													if (map[signY1, signX1] <= 0xea || map[signY1, signX1] >= 0xf4)
														map[signY1, signX1] = map[signY, signX];
													island[signY1, signX1] = island[signY, signX];
												}
											break;
										}
									}
								
								map[y - 4, x - 4] = 0xf5;

								int byteToUse = 0x3be1c + (3 * lnI);
								romData[byteToUse] = (byte)x;
								romData[byteToUse + 1] = (byte)y;

								romData[0x23e83] = romData[0x2365b] = (byte)x; // Second romData:  Needed for comparison to make sure you did the Alex thing
								romData[0x23e84] = romData[0x23661] = (byte)y;
							}
							else
								lnI--;
						}
						else if (lnI == 59) // Gottside Island
						{
							bool baramosLegal = true;
							for (int lnJ = x - 2; lnJ <= x + 2; lnJ++)
								for (int lnK = y - 2; lnK <= y + 2; lnK++)
								{
									if (island[lnK, lnJ] != maxLake || map[lnK, lnJ] != 0x00)
										baramosLegal = false;
								}

							if (baramosLegal && towerCheck(y, x))
							{
								for (int lnJ = -2; lnJ <= 2; lnJ++)
									for (int lnK = -2; lnK <= 2; lnK++)
										island[y + lnJ, x + lnK] = 12001;

								for (int lnJ = -2; lnJ <= 2; lnJ++)
									for (int lnK = -2; lnK <= 2; lnK++)
										map[y + lnJ, x + lnK] = 0x06;

								for (int lnJ = -1; lnJ <= 1; lnJ++)
									for (int lnK = -1; lnK <= 1; lnK++)
										map[y + lnJ, x + lnK] = 0x02;

								map[y, x] = 0x01;

								romData[0x79c2f] = (byte)(x - 1);
								romData[0x79c31] = 3;
								romData[0x79c38] = (byte)(y - 1);
								romData[0x79c3a] = 3;

								romData[0x79c5a] = (byte)x;
								romData[0x79c5c] = (byte)y;
							}
							else
								lnI--;
						}
						break;
					case "X":
						continue;
				}
			}

			for (int lnI = 0; lnI < locTypes.Length; lnI++)
			{
				int x = 300;
				int y = 300;
				if (lnI == 1) { x = midenX[1]; y = midenY[1]; } // Burland
				else if (lnI == 2) { x = midenX[3]; y = midenY[3]; } // Santeem
				else if (lnI == 3) { x = midenX[9]; y = midenY[9]; } // Branca
				else if (lnI == 6) { x = midenX[5]; y = midenY[5]; } // Endor
				else if (lnI == 10) { x = midenX[2]; y = midenY[2]; } // Izmit
				else if (lnI == 11) { x = midenX[4]; y = midenY[4]; } // Frenor
				else if (lnI == 12) { x = midenX[6]; y = midenY[6]; } // Lakanaba
				else if (lnI == 21) { x = midenX[10]; y = midenY[10]; } // Konenbur
				else if (lnI == 26) { x = midenX[8]; y = midenY[8]; } // Monbaraba
				else if (lnI == 28) { x = midenX[7]; y = midenY[7]; } // Silver Statuette Cave
				else if (locIslands[lnI] == -100)
				{
					continue;
				}
				else
				{
					// Subtract 6 for room
					x = 6 + r1.Next() % (oSmallMap ? 128 - 6 - 6 : 256 - 6 - 6);
					y = 6 + r1.Next() % (oSmallMap ? 128 - 6 - 6 : 256 - 6 - 6);
				}

				switch (locTypes[lnI])
				{
					case "W": // Tower
						if (validPlot(y - 1, x - 1, 4, 3, locIslands[lnI] <= 10 ? new int[] { maxIsland[locIslands[lnI]] } : islands.ToArray()) && legalPlacement(y, x) && reachable(y, x, !landLocs.Contains(lnI),
							locIslands[lnI] <= 10 ? midenX[locIslands[lnI]] : midenX[10], locIslands[lnI] <= 10 ? midenY[locIslands[lnI]] : midenY[10], maxLake, false))
						{
							map[y + 0, x + 0] = 0xed;
							map[y + 1, x + 0] = 0xf1;

							int byteToUse = 0x3be1c + (3 * lnI);
							romData[byteToUse] = (byte)x;
							romData[byteToUse + 1] = (byte)(y + 1);

							if (lnI == 49)
							{
								romData[0x7afea] = (byte)x;
								romData[0x7afeb] = (byte)y;
							}
						}
						else
							lnI--;

						break;
					case "?":
						if (lnI == 49) // Loch Tower
						{
							if (validPlot(y - 2, x, 6, 1, new int[] { maxIsland[1], maxIsland[2] })
								&& (reachable(y, x, !landLocs.Contains(lnI), midenX[0], midenY[0], maxLake, false) || reachable(y, x, !landLocs.Contains(lnI), midenX[1], midenY[1], maxLake, false)))
							{
								int zoneSegment = (oSmallMap ? 8 : 16);
								bool baramosLegal = true;
								for (int lnY = -2; lnY <= 3; lnY++)
									for (int lnX = -3; lnX <= 2; lnX++)
									{
										if (map[y + lnY, x + lnX] >= 0xe8)
											baramosLegal = false;
										if (zone[(y + lnY) / zoneSegment, (x + lnX) / zoneSegment] / 1000 != 1 && zone[(y + lnY) / zoneSegment, (x + lnX) / zoneSegment] / 1000 != 2)
											baramosLegal = false;
									}
								if (baramosLegal && legalPlacement(y + 1, x) && ((island[y, x - 3] != maxIsland[1] && island[y, x + 3] != maxIsland[1]) || (island[y, x - 3] != maxIsland[2] && island[y, x + 3] != maxIsland[2])))
								{
									if (!oCh1Moat)
									{
										map[y + 0, x - 1] = map[y + 1, x - 1] = 0x01;
										map[y + 0, x - 2] = map[y + 1, x - 2] = 0x00;
										map[y + 0, x + 1] = map[y + 1, x + 1] = 0x00;
										map[y + 0, x - 3] = map[y + 1, x - 3] = map[y - 1, x - 3] = map[y + 2, x - 3] = 0x01;
										map[y + 0, x + 2] = map[y + 1, x + 2] = map[y - 1, x + 2] = map[y + 2, x + 2] = 0x01;
										map[y - 1, x - 2] = map[y - 1, x - 1] = map[y - 1, x] = map[y - 1, x + 1] = 0x00;
										map[y + 2, x - 2] = map[y + 2, x - 1] = map[y + 2, x] = map[y + 2, x + 1] = 0x00;
										map[y - 2, x - 3] = map[y - 2, x - 2] = map[y - 2, x - 1] = map[y - 2, x] = map[y - 2, x + 1] = map[y - 2, x + 2] = 0x01;
										map[y + 3, x - 3] = map[y + 3, x - 2] = map[y + 3, x - 1] = map[y + 3, x] = map[y + 3, x + 1] = map[y + 3, x + 2] = 0x01;
									}
									map[y + 0, x + 0] = 0xed;
									map[y + 1, x + 0] = 0xf1;

									int byteToUse = 0x3be1c + (3 * lnI);
									romData[byteToUse] = (byte)x;
									romData[byteToUse + 1] = (byte)(y + 1);

									// Return spot via Flying Shoes
									romData[0x7afea] = (byte)x;
									romData[0x7afeb] = (byte)y;
								}
								else
									lnI--;
							}
							else
								lnI--;
						}
						else if (lnI == 54) // Elfville
						{
							if (validPlot(y, x, 5, 5, islands.ToArray()) && legalPlacement(y + 2, x + 2) && reachable(y, x, true, midenX[10], midenY[10], maxLake, false))
							{
								for (int lnJ = 1; lnJ <= 3; lnJ++)
									for (int lnK = 1; lnK <= 3; lnK++)
										map[y + lnJ, x + lnK] = 0x05;

								map[y + 2, x + 2] = 0x04;

								int byteToUse = 0x3be1c + (3 * lnI);
								romData[byteToUse] = (byte)(x + 2);
								romData[byteToUse + 1] = (byte)(y + 2);

								romData[0x23e86] = (byte)(x + 2);
								romData[0x23e87] = (byte)(y + 2);
							}
							else
								lnI--;
						}

						break;
				}
			}

			// Stop the "The Ship Arrived In Endor!" in Chapter 4.
			for (int lnI = 0; lnI < 7; lnI++)
			romData[0x7cf5e + lnI] = 0xea;
			romData[0x6eb8e] = 0x11; // Skip the ship ride by making the location comparison Haville instead of Endor upon entering a location.  It runs as soon as the ship leaves town.

			List<int> part1 = new List<int>() { 4 };
			List<int> part2 = new List<int>() { 5, 6 };
			List<int> part3 = new List<int>() { 7, 8 };
			List<int> part4 = new List<int>() { 9, 10, 11 };
			List<int> part5 = new List<int>() { 12 };
			List<int> part6 = new List<int>() { 20, 21 };
			List<int> part7 = new List<int>() { 23 };
			List<int> part8 = new List<int>() { 13, 14, 15, 16, 17, 18 };
			List<int> part9 = new List<int>() { 24, 25, 26, 27, 28, 29 };
			List<int> part10 = new List<int>() { 30, 31, 32 };
			List<int> part11 = new List<int>() { 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51 };

			int[,] monsterZones = new int[16, 16];
			for (int lnI = 0; lnI < 16; lnI++)
				for (int lnJ = 0; lnJ < 16; lnJ++)
					monsterZones[lnI, lnJ] = 0xff;

			for (int mzX = 0; mzX < 16; mzX++)
				for (int mzY = 0; mzY < 16; mzY++)
				{
					if (zone[mzY, mzX] / 1000 == 1)
						monsterZones[mzY, mzX] = part1[r1.Next() % part1.Count];
					else if (zone[mzY, mzX] / 1000 == 2)
						monsterZones[mzY, mzX] = part2[r1.Next() % part2.Count];
					else if (zone[mzY, mzX] / 1000 == 3)
						monsterZones[mzY, mzX] = part3[r1.Next() % part3.Count];
					else if (zone[mzY, mzX] / 1000 == 4)
						monsterZones[mzY, mzX] = part4[r1.Next() % part4.Count];
					else if (zone[mzY, mzX] / 1000 == 5)
						monsterZones[mzY, mzX] = part5[r1.Next() % part5.Count];
					else if (zone[mzY, mzX] / 1000 == 6)
						monsterZones[mzY, mzX] = part6[r1.Next() % part6.Count];
					else if (zone[mzY, mzX] / 1000 == 7)
						monsterZones[mzY, mzX] = part7[r1.Next() % part7.Count];
					else if (zone[mzY, mzX] / 1000 == 8)
						monsterZones[mzY, mzX] = part8[r1.Next() % part8.Count];
				}

			int[,] monsterZones2 = new int[16, 16];
			for (int lnI = 0; lnI < 16; lnI++)
				for (int lnJ = 0; lnJ < 16; lnJ++)
					monsterZones2[lnI, lnJ] = 0xff;

			for (int mzX = 0; mzX < 16; mzX++)
				for (int mzY = 0; mzY < 16; mzY++)
				{
					if (zone[mzY, mzX] / 1000 == 9 || zone[mzY, mzX] / 1000 == 5)
						monsterZones2[mzY, mzX] = part9[r1.Next() % part9.Count];
					else if (zone[mzY, mzX] / 1000 == 10 || zone[mzY, mzX] / 1000 == 6)
						monsterZones2[mzY, mzX] = part10[r1.Next() % part10.Count];
					else
						monsterZones2[mzY, mzX] = part11[r1.Next() % part11.Count];

					monsterZones2[mzY, mzX] += 0x80 * (r1.Next() % 2);
				}

			bool badMap = true;
			bool compressed = false;
			while (badMap)
			{
				// Now let's enter all of this into the ROM...
				int lnPointer = 0x8cee;

				for (int lnI = 0; lnI <= 256; lnI++) // <---- There is a final pointer for lnI = 256, probably indicating the conclusion of the map.
				{
					romData[0x2e5a0 + (lnI * 4)] = (byte)(lnPointer % 256);
					romData[0x2e5a0 + (lnI * 4) + 1] = (byte)(lnPointer / 256);

					int lnJ = 0;
					int lineBytes = 0;
					int midBytes = 0;
					int limit = 128;
					while (lnI < 256 && lnJ < 256)
					{
						if (map[lnI, lnJ] >= 0 && map[lnI, lnJ] <= 7)
						{
							int tileNumber = 0;
							int numberToMatch = map[lnI, lnJ];
							while (lnJ < limit && tileNumber < (numberToMatch == 7 ? 8 : 32) && map[lnI, lnJ] == numberToMatch)
							{
								tileNumber++;
								lnJ++;
							}
							romData[lnPointer + 0x24010] = (byte)((0x20 * numberToMatch) + (tileNumber - 1));
						}
						else
						{
							romData[lnPointer + 0x24010] = (byte)map[lnI, lnJ];
							lnJ++;
						}

						lnPointer++;
						lineBytes++;
						if (lnJ <= 128 && limit == 128)
						{
							midBytes++;
							if (lnJ == 128)
								limit = 256;
						}
					}
					romData[0x2e5a0 + (lnI * 4) + 2] = (byte)midBytes;
					romData[0x2e5a0 + (lnI * 4) + 3] = (byte)lineBytes;
				}
				if (compressed) badMap = false;

				//lnPointer = lnPointer;
				if (lnPointer > 0xa590)
				{
					MessageBox.Show("WARNING:  The map might have taken too much ROM space... (" + (lnPointer - 0xa590).ToString() + " over)");
					compressed = true;
					// Might have to compress further to remove one byte stuff
					// Must compress the map by getting rid of further 1 byte lakes
				}
				else
					badMap = false;
			}

			//// Ensure monster zones are 8x8
			if (oSmallMap)
			{
				romData[0x61c79] = 0x85;
				romData[0x61c7a] = 0x06;
				romData[0x61c7b] = 0xa5;
				romData[0x61c7c] = 0x43;
				romData[0x61c7d] = 0x0a;
				romData[0x61c7e] = 0x29;
				romData[0x61c7f] = 0xf0;
			}

			// Use the early chapter zones regardless of Chapter.  (Chapter 3 has a special zone set, but it's only 128 bytes long, not the usual 256.)
			romData[0x62254] = 0xa4;

			// Enter monster zones
			for (int lnI = 0; lnI < 16; lnI++)
				for (int lnJ = 0; lnJ < 16; lnJ++)
				{
					if (monsterZones[lnI, lnJ] == 0xff)
						monsterZones[lnI, lnJ] = 0;
					romData[0x624cd + (lnI * 16) + lnJ] = (byte)monsterZones[lnI, lnJ];
				}

			for (int lnI = 0; lnI < 16; lnI++)
				for (int lnJ = 0; lnJ < 16; lnJ++)
				{
					if (monsterZones2[lnI, lnJ] == 0xff)
						monsterZones2[lnI, lnJ] = 0;
					romData[0x625cd + (lnI * 16) + lnJ] = (byte)monsterZones2[lnI, lnJ];
				}

			// Remove "The Ship has arrived at Endor!" messages... I hope.
			romData[0x3cf5b] = romData[0x7cf5b] = 0x00;

			return true;
		}

		private void markZoneSides()
		{
			for (int x = 0; x < 16; x++)
				for (int y = 0; y < 16; y++)
				{
					// 1 = north, 2 = east, 4 = south, 8 = west
					if (y == 0) zone[y, x] += 1;
					else if (zone[y - 1, x] / 1000 != zone[y, x] / 1000) zone[y, x] += 1;

					if (x == 15) zone[y, x] += 2;
					else if (zone[y, x + 1] / 1000 != zone[y, x] / 1000) zone[y, x] += 2;

					if (y == 15) zone[y, x] += 4;
					else if (zone[y + 1, x] / 1000 != zone[y, x] / 1000) zone[y, x] += 4;

					if (x == 0) zone[y, x] += 8;
					else if (zone[y, x - 1] / 1000 != zone[y, x] / 1000) zone[y, x] += 8;
				}
		}

		private void generateZoneMap(int zoneToUse, int islandSize, Random r1)
		{
			int xMax = (zoneToUse != -1000 ? (oSmallMap ? 128 : 256) : (oSmallMap ? 80 : 136)) - 7;
			int yMax = (zoneToUse != -1000 ? (oSmallMap ? 128 : 256) : (oSmallMap ? 80 : 132)) - 7;
			int yMin = 6;
			int xMin = 6;

			int[] terrainTypes = { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 7 };

			for (int lnI = 0; lnI < 100; lnI++)
			{
				int swapper1 = r1.Next() % terrainTypes.Length;
				int swapper2 = r1.Next() % terrainTypes.Length;
				int temp = terrainTypes[swapper1];
				terrainTypes[swapper1] = terrainTypes[swapper2];
				terrainTypes[swapper2] = temp;
			}

			int lnMarker = -1;
			int totalLand = 0;
			int attempts = 0;

			while (totalLand < islandSize && attempts < 200000)
			{
				attempts++;
				lnMarker++;
				lnMarker = (lnMarker >= terrainTypes.Length ? 0 : lnMarker);
				int sizeToUse = (r1.Next() % 400) + 150;
				//if (terrainTypes[lnMarker] == 5) sizeToUse /= 2;

				List<int> points = new List<int> { (r1.Next() % (xMax - xMin)) + xMin, (r1.Next() % (yMax - yMin)) + yMin };
				if (validPoint(points[0], points[1], zoneToUse))
				{
					while (sizeToUse > 0)
					{
						List<int> newPoints = new List<int>();
						for (int lnI = 0; lnI < points.Count; lnI += 2)
						{
							int lnX = points[lnI];
							int lnY = points[lnI + 1];

							int direction = (r1.Next() % 16);
							if (zoneToUse != -1000)
							{
								map[lnY, lnX] = terrainTypes[lnMarker];
								island[lnY, lnX] = (terrainTypes[lnMarker] == 6 ? -1 - zoneToUse : zoneToUse);
							}
							else
							{
								map2[lnY, lnX] = terrainTypes[lnMarker];
								island2[lnY, lnX] = (terrainTypes[lnMarker] == 6 ? -1 : 0);
							}

							// 1 = North, 2 = east, 4 = south, 8 = west
							if (direction % 8 >= 4 && lnY <= yMax)
							{
								if (validPoint(lnX, lnY + 1, zoneToUse))
								{
									if (zoneToUse == -1000)
									{
										if (map2[lnY + 1, lnX] == 0)
											totalLand++;
										map2[lnY + 1, lnX] = terrainTypes[lnMarker];
										island2[lnY + 1, lnX] = (terrainTypes[lnMarker] == 6 ? -1 : 0);
									}
									else
									{
										if (map[lnY + 1, lnX] == 0)
											totalLand++;
										map[lnY + 1, lnX] = terrainTypes[lnMarker];
										island[lnY + 1, lnX] = (terrainTypes[lnMarker] == 6 ? -1 - zoneToUse : zoneToUse);
									}

									newPoints.Add(lnX);
									newPoints.Add(lnY + 1);
								}
							}
							if (direction % 2 >= 1 && lnY >= yMin)
							{
								if (validPoint(lnX, lnY - 1, zoneToUse))
								{
									if (zoneToUse == -1000)
									{
										if (map2[lnY - 1, lnX] == 0)
											totalLand++;
										map2[lnY - 1, lnX] = terrainTypes[lnMarker];
										island2[lnY - 1, lnX] = (terrainTypes[lnMarker] == 6 ? -1 : 0);
									}
									else
									{
										if (map[lnY - 1, lnX] == 0)
											totalLand++;
										map[lnY - 1, lnX] = terrainTypes[lnMarker];
										island[lnY - 1, lnX] = (terrainTypes[lnMarker] == 6 ? -1 - zoneToUse : zoneToUse);
									}
									newPoints.Add(lnX);
									newPoints.Add(lnY - 1);
								}
							}
							if (direction % 4 >= 2 && lnX <= xMax)
							{
								if (validPoint(lnX + 1, lnY, zoneToUse))
								{
									if (zoneToUse == -1000)
									{
										if (map2[lnY, lnX + 1] == 0)
											totalLand++;
										map2[lnY, lnX + 1] = terrainTypes[lnMarker];
										island2[lnY, lnX + 1] = (terrainTypes[lnMarker] == 6 ? -1 : 0);
									}
									else
									{
										if (map[lnY, lnX + 1] == 0)
											totalLand++;
										map[lnY, lnX + 1] = terrainTypes[lnMarker];
										island[lnY, lnX + 1] = (terrainTypes[lnMarker] == 6 ? -1 - zoneToUse : zoneToUse);
									}
									newPoints.Add(lnX + 1);
									newPoints.Add(lnY);
								}
							}
							if (direction % 16 >= 8 && lnX >= xMin)
							{
								if (validPoint(lnX - 1, lnY, zoneToUse))
								{
									if (zoneToUse == -1000)
									{
										if (map2[lnY, lnX - 1] == 0)
											totalLand++;
										map2[lnY, lnX - 1] = terrainTypes[lnMarker];
										island2[lnY, lnX - 1] = (terrainTypes[lnMarker] == 6 ? -1 : 0);
									}
									else
									{
										if (map[lnY, lnX - 1] == 0)
											totalLand++;
										map[lnY, lnX - 1] = terrainTypes[lnMarker];
										island[lnY, lnX - 1] = (terrainTypes[lnMarker] == 6 ? -1 - zoneToUse : zoneToUse);
									}
									newPoints.Add(lnX - 1);
									newPoints.Add(lnY);
								}
							}

							int takeaway = 1 + (direction > 8 ? 1 : 0) + (direction % 8 > 4 ? 1 : 0) + (direction % 4 > 2 ? 1 : 0) + (direction % 2 > 1 ? 1 : 0);
							sizeToUse--;
						}
						if (sizeToUse <= 0) break;
						if (newPoints.Count != 0)
							points = newPoints;
					}
				}
			}

			// Fill in water...
			List<int> land = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
			if (zoneToUse != -1000)
			{
				for (int lnY = 0; lnY < 256; lnY++)
					for (int lnX = 0; lnX < 256; lnX++)
					{
						if (island[lnY, lnX] == zoneToUse && island[lnY, lnX + 1] == zoneToUse && island[lnY, lnX + 2] == zoneToUse && island[lnY, lnX + 3] == zoneToUse)
						{
							if (map[lnY, lnX] == map[lnY, lnX + 2] && map[lnY, lnX] != map[lnY, lnX + 1])
							{
								map[lnY, lnX + 1] = map[lnY, lnX];
								island[lnY, lnX + 1] = island[lnY, lnX];
							}
							if (lnX < 254 && land.Contains(map[lnY, lnX]) && !land.Contains(map[lnY, lnX + 1]) && !land.Contains(map[lnY, lnX + 2]) && land.Contains(map[lnY, lnX + 3]))
							{
								map[lnY, lnX + 1] = map[lnY, lnX];
								map[lnY, lnX + 2] = map[lnY, lnX + 3];
								island[lnY, lnX + 1] = island[lnY, lnX];
								island[lnY, lnX + 2] = island[lnY, lnX + 3];
							}
						}
					}

				markIslands(zoneToUse);
			}
			else
			{
				for (int lnY = 0; lnY < 136; lnY++)
					for (int lnX = 0; lnX < 156; lnX++)
					{
						if (map2[lnY, lnX] == map2[lnY, lnX + 2] && map2[lnY, lnX] != map2[lnY, lnX + 1])
						{
							map2[lnY, lnX + 1] = map2[lnY, lnX];
							island2[lnY, lnX + 1] = island2[lnY, lnX];
						}
						if (lnX < 149 && land.Contains(map2[lnY, lnX]) && !land.Contains(map2[lnY, lnX + 1]) && !land.Contains(map2[lnY, lnX + 2]) && land.Contains(map2[lnY, lnX + 3]))
						{
							map2[lnY, lnX + 1] = map2[lnY, lnX];
							map2[lnY, lnX + 2] = map2[lnY, lnX + 3];
							island2[lnY, lnX + 1] = island2[lnY, lnX];
							island2[lnY, lnX + 2] = island2[lnY, lnX + 3];
						}
					}
			}
		}

		private void smoothMap()
		{
			// Remove one byte lands
			for (int lnX = 0; lnX < 254; lnX++)
				for (int lnY = 0; lnY < 256; lnY++)
				{
					if (map[lnY, lnX] != map[lnY, lnX + 1] && map[lnY, lnX + 1] != map[lnY, lnX + 2] && island[lnY, lnX] == island[lnY, lnX + 2])
					{
						map[lnY, lnX + 1] = map[lnY, lnX];
						island[lnY, lnX + 1] = island[lnY, lnX];
					}
				}

			int smoothRequirement = 10;
			bool badMap = true;

			while (badMap)
			{
				// Let's PRETEND to enter this into the ROM...
				int lnPointer = 0x8cee;

				for (int lnI = 0; lnI <= 256; lnI++) // <---- There is a final pointer for lnI = 256, probably indicating the conclusion of the map.
				{
					int lnJ = 0;
					while (lnI < 256 && lnJ < 256)
					{
						if (map[lnI, lnJ] >= 0 && map[lnI, lnJ] <= 7)
						{
							int tileNumber = 0;
							int numberToMatch = map[lnI, lnJ];
							while (lnJ < 256 && tileNumber < (numberToMatch == 7 ? 8 : 32) && map[lnI, lnJ] == numberToMatch)
							{
								tileNumber++;
								lnJ++;
							}
							lnPointer++;
						}
						else
						{
							lnPointer++;
							lnJ++;
						}
					}
				}
				//lnPointer = lnPointer;
				if (lnPointer <= 0xa590 - 1000)
					badMap = false;
				else // Time to remove small areas of stuff to hopefully compress the map further.
				{
					//MessageBox.Show("Map too big; " + (lnPointer - (0x9a94 - 256)).ToString() + " bytes too big");

					int lastTile = 0x00;
					int lastIsland = island[0, 0];
					for (int lnY = 0; lnY < 255; lnY++)
						for (int lnX = 0; lnX < 255; lnX++)
						{
							smoothPlot(lnX, lnY, smoothRequirement, lastTile, lastIsland);
							lastTile = map[lnY, lnX];
							lastIsland = island[lnY, lnX];
						}

					smoothRequirement += 10;
				}
			}
		}

		private void smoothMap2()
		{
			// Remove one byte lands
			for (int lnX = 0; lnX < 156; lnX++)
				for (int lnY = 0; lnY < 139; lnY++)
				{
					if (map2[lnY, lnX] != map2[lnY, lnX + 1] && map2[lnY, lnX + 1] != map2[lnY, lnX + 2] && island2[lnY, lnX] == island2[lnY, lnX + 2])
					{
						map2[lnY, lnX + 1] = map2[lnY, lnX];
						island2[lnY, lnX + 1] = island2[lnY, lnX];
					}
				}

			int smoothRequirement = 10;
			bool badMap = true;

			while (badMap)
			{
				// Let's PRETEND to enter this into the ROM...
				int lnPointer = 0x9bab;

				for (int lnI = 0; lnI <= 138; lnI++) // <---- There is a final pointer for lnI = 256, probably indicating the conclusion of the map2.
				{
					int lnJ = 0;
					while (lnI < 139 && lnJ < 158)
					{
						if (map2[lnI, lnJ] >= 0 && map2[lnI, lnJ] <= 7)
						{
							int tileNumber = 0;
							int numberToMatch = map2[lnI, lnJ];
							while (lnJ < 158 && tileNumber < (numberToMatch == 7 ? 8 : 32) && map2[lnI, lnJ] == numberToMatch)
							{
								tileNumber++;
								lnJ++;
							}
							lnPointer++;
						}
						else
						{
							lnPointer++;
							lnJ++;
						}
					}
				}
				//lnPointer = lnPointer;
				if (lnPointer <= 0xa3ee - 80)
					badMap = false;
				else // Time to remove small areas of stuff to hopefully compress the map further.
				{
					//MessageBox.Show("Map too big; " + (lnPointer - (0x9a94 - 256)).ToString() + " bytes too big");

					int lastTile = 0x00;
					int lastIsland = island2[0, 0];
					for (int lnY = 0; lnY < 139; lnY++)
						for (int lnX = 0; lnX < 158; lnX++)
						{
							smoothPlot2(lnX, lnY, smoothRequirement, lastTile, lastIsland);
							lastTile = map2[lnY, lnX];
							lastIsland = island2[lnY, lnX];
						}

					smoothRequirement += 10;
				}
			}

		}

		private void smoothPlot(int initX, int initY, int minimum, int fillTile, int fillIsland)
		{
			//if (y == 30 && x == 137)
			//    y = y;

			int x = initX;
			int y = initY;

			bool[,] plotted = new bool[256, 256];
			int landTile = map[y, x];

			bool first = true;
			List<int> toPlot = new List<int>();
			int plots = 0;
			int toFill = 0;
			while (toFill < 2)
			{
				while (first || toPlot.Count != 0)
				{
					if (!first)
					{
						y = toPlot[0];
						toPlot.RemoveAt(0);
						x = toPlot[0];
						toPlot.RemoveAt(0);
					}
					else
						first = false;

					if (toFill == 1)
					{
						map[y, x] = fillTile;
						island[y, x] = fillIsland;
					}

					for (int dir = 0; dir < 5; dir++)
					{
						int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
						dirX = (dirX == 256 ? 0 : dirX == -1 ? 255 : dirX);
						int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
						dirY = (dirY == 256 ? 0 : dirY == -1 ? 255 : dirY);

						if (map[dirY, dirX] == landTile && !plotted[dirY, dirX])
						{
							plots++;
							plotted[dirY, dirX] = true;

							if (plots > minimum)
								return;

							if (dir != 0)
							{
								toPlot.Add(dirY);
								toPlot.Add(dirX);
							}
						}
					}
				}

				toFill++;
				x = initX;
				y = initY;
				first = true;
			}
		}

		private void smoothPlot2(int initX, int initY, int minimum, int fillTile, int fillIsland)
		{
			//if (y == 30 && x == 137)
			//    y = y;

			int x = initX;
			int y = initY;

			bool[,] plotted = new bool[139, 158];
			int landTile = map2[y, x];

			bool first = true;
			List<int> toPlot = new List<int>();
			int plots = 0;
			int toFill = 0;
			while (toFill < 2)
			{
				while (first || toPlot.Count != 0)
				{
					if (!first)
					{
						y = toPlot[0];
						toPlot.RemoveAt(0);
						x = toPlot[0];
						toPlot.RemoveAt(0);
					}
					else
						first = false;

					if (toFill == 1)
					{
						map2[y, x] = fillTile;
						island2[y, x] = fillIsland;
					}

					for (int dir = 0; dir < 5; dir++)
					{
						int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
						dirX = (dirX == 158 ? 0 : dirX == -1 ? 157 : dirX);
						int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
						dirY = (dirY == 139 ? 0 : dirY == -1 ? 138 : dirY);

						if (map2[dirY, dirX] == landTile && !plotted[dirY, dirX])
						{
							plots++;
							plotted[dirY, dirX] = true;

							if (plots > minimum)
								return;

							if (dir != 0)
							{
								toPlot.Add(dirY);
								toPlot.Add(dirX);
							}
						}
					}
				}

				toFill++;
				x = initX;
				y = initY;
				first = true;
			}
		}

		private bool validPoint(int x, int y, int zoneToUse)
		{
			if (zoneToUse == -1000) return true;
			int zoneSize = (oSmallMap ? 8 : 16);
			// Establish zone
			int zoneX = x / zoneSize;
			int zoneY = y / zoneSize;
			int zoneSides = zone[zoneY, zoneX] % 1000;
			//if (zone[zoneY, zoneX] % 1000 != 0) return false;
			if (zone[zoneY, zoneX] / 1000 != zoneToUse / 1000) return false;
			// 1 = north, 2 = east, 4 = south, 8 = west
			if (y % zoneSize == 0 && zoneSides % 2 == 1) return false;
			if (x % zoneSize == zoneSize - 1 && zoneSides % 4 >= 2) return false;
			if (y % zoneSize == zoneSize - 1 && zoneSides % 8 >= 4) return false;
			if (x % zoneSize == 0 && zoneSides % 16 >= 8) return false;

			return true;
		}

		private void markIslands(int zoneToUse)
		{
			if (zoneToUse != -1000)
			{
				// We should mark islands and inaccessible land...
				int landNumber = zoneToUse + 1;
				int maxLand = -2;

				int maxLandPlots = 0;
				int lastIsland = 0;
				for (int lnI = 0; lnI < 256; lnI++)
					for (int lnJ = 0; lnJ < 256; lnJ++)
					{
						if (island[lnI, lnJ] == zoneToUse && map[lnI, lnJ] != 0x06)
						{
							int plots = landPlot(landNumber, lnI, lnJ, zoneToUse);
							if (plots > maxLandPlots)
							{
								maxLandPlots = plots;
								maxLand = landNumber;
							}
							islands.Add(landNumber);
							landNumber++;

							lastIsland = island[lnI, lnJ];
						}
					}

				maxIsland[zoneToUse / 1000] = maxLand;
			}
			else
			{
				// We should mark islands and inaccessible land...
				int landNumber = 1;

				for (int lnI = 0; lnI < 139; lnI++)
					for (int lnJ = 0; lnJ < 158; lnJ++)
					{
						if (island2[lnI, lnJ] == 0 && map2[lnI, lnJ] != 0x06)
						{
							int plots = landPlot(landNumber, lnI, lnJ, zoneToUse);
							landNumber++;
						}
					}
			}
		}

		private void resetIslands()
		{
			for (int y = 0; y < 256; y++)
				for (int x = 0; x < 256; x++)
				{
					if (island[y, x] != 200 && island[y, x] != -1)
					{
						island[y, x] /= 1000;
						island[y, x] *= 1000;
					}
				}

			islands.Clear();

			markIslands(1000);
			markIslands(2000);
			markIslands(3000);
			markIslands(4000);
			markIslands(5000);
			markIslands(6000);
			markIslands(7000);
			markIslands(8000);
			markIslands(9000);
			markIslands(10000);
			markIslands(0);
		}

		private void createBridges(Random r1)
		{
			List<BridgeList> bridgePossible = new List<BridgeList>();
			List<islandLinks> islandPossible = new List<islandLinks>();
			// Create bridges for points two spaces or less from two distinctly numbered islands.  Extend islands if there is interference.
			// 0x00 = Water (was 0x04 in DW2)
			// 0x06 = Mountain (was 0x05 in DW2)
			for (int y = 1; y < 252; y++)
				for (int x = 1; x < 252; x++)
				{
					if (y == 63 && x == 8) map[y, x] = map[y, x];
					if (map[y, x] == 0x06 || map[y, x] == 0x00) continue;

					for (int lnI = 2; lnI <= 4; lnI++)
					{
						if (island[y, x] != island[y + lnI, x] && island[y, x] / 1000 == island[y + lnI, x] / 1000 && map[y + lnI, x] != 0x00 && map[y + lnI, x] != 0x06)
						{
							bool fail = false;
							for (int lnJ = 1; lnJ < lnI; lnJ++)
							{
								if (map[y + lnJ, x] == 0x00)
								{
									map[y + lnJ, x - 1] = 0x00; map[y + lnJ, x + 1] = 0x00;
									island[y + lnJ, x - 1] = 0x00; island[y + lnJ, x + 1] = 0x00;
								}
								else
								{
									fail = true;
								}
							}
							if (!fail)
							{
								bridgePossible.Add(new BridgeList(x, y, true, lnI, island[y, x], island[y + lnI, x]));
								if (islandPossible.Where(c => c.island1 == island[y, x] && c.island2 == island[y + lnI, x]).Count() == 0)
									islandPossible.Add(new islandLinks(island[y, x], island[y + lnI, x]));
							}
						}

						if (island[y, x] != island[y, x + lnI] && island[y, x] / 1000 == island[y, x + lnI] / 1000 && map[y, x + lnI] != 0x00 && map[y, x + lnI] != 0x06)
						{
							bool fail = false;
							for (int lnJ = 1; lnJ < lnI; lnJ++)
							{
								if (map[y, x + lnJ] == 0x00)
								{
									map[y - 1, x + lnJ] = 0x00; map[y + 1, x + lnJ] = 0x00;
									island[y - 1, x + lnJ] = 200; island[y + 1, x + lnJ] = 200;
								}
								else
								{
									fail = true;
								}
							}
							if (!fail)
							{
								bridgePossible.Add(new BridgeList(x, y, false, lnI, island[y, x], island[y, x + lnI]));
								if (islandPossible.Where(c => c.island1 == island[y, x] && c.island2 == island[y, x + lnI]).Count() == 0)
									islandPossible.Add(new islandLinks(island[y, x], island[y, x + lnI]));
							}
						}
					}
				}

			foreach (islandLinks islandLink in islandPossible)
			{
				List<BridgeList> test = bridgePossible.Where(c => c.island1 == islandLink.island1 && c.island2 == islandLink.island2).ToList();
				// Choose one bridge out of the possibilities
				BridgeList bridgeToBuild = test[r1.Next() % test.Count];
				for (int lnI = 1; lnI <= bridgeToBuild.distance - 1; lnI++)
				{
					if (bridgeToBuild.south)
					{
						if (bridgeToBuild.y + lnI > 250)
							break;
						map[bridgeToBuild.y + lnI, bridgeToBuild.x] = (lnI == bridgeToBuild.distance - 1 ? 0x07 : map[bridgeToBuild.y, bridgeToBuild.x]);
						island[bridgeToBuild.y + lnI, bridgeToBuild.x] = bridgeToBuild.island1;

						if (map[bridgeToBuild.y + lnI + 1, bridgeToBuild.x] == 0x00 && lnI == bridgeToBuild.distance - 1)
						{
							bridgeToBuild.distance++;
							map[bridgeToBuild.y + lnI + 1, bridgeToBuild.x - 1] = 0x00; map[bridgeToBuild.y + lnI + 1, bridgeToBuild.x + 1] = 0x00;
							island[bridgeToBuild.y + lnI + 1, bridgeToBuild.x - 1] = 0x00; island[bridgeToBuild.y + lnI + 1, bridgeToBuild.x + 1] = 0x00;
						}
					}
					else
					{
						if (bridgeToBuild.x + lnI > 250)
							break;
						map[bridgeToBuild.y, bridgeToBuild.x + lnI] = (lnI == bridgeToBuild.distance - 1 ? 0x07 : map[bridgeToBuild.y, bridgeToBuild.x]);
						island[bridgeToBuild.y, bridgeToBuild.x + lnI] = bridgeToBuild.island1;

						if (map[bridgeToBuild.y, bridgeToBuild.x + lnI + 1] == 0x00 && lnI == bridgeToBuild.distance - 1)
						{
							bridgeToBuild.distance++;
							map[bridgeToBuild.y - 1, bridgeToBuild.x + lnI + 1] = 0x00; map[bridgeToBuild.y + 1, bridgeToBuild.x + lnI + 1] = 0x00;
							island[bridgeToBuild.y - 1, bridgeToBuild.x + lnI + 1] = 200; island[bridgeToBuild.y + 1, bridgeToBuild.x + lnI + 1] = 200;
						}
					}
				}
			}
		}

		private class islandLinks
		{
			public int island1;
			public int island2;

			public islandLinks(int pI1, int pI2)
			{
				island1 = pI1; island2 = pI2;
			}
		}

		private class BridgeList
		{
			public int x;
			public int y;
			public bool south;
			public int distance;
			public int island1;
			public int island2;

			public BridgeList(int pX, int pY, bool pS, int pDist, int pI1, int pI2)
			{
				x = pX; y = pY; south = pS; distance = pDist; island1 = pI1; island2 = pI2;
			}
		}

		private bool createZone(int zoneNumber, int size, bool rectangle, Random r1, int adjacentZone = -1)
		{
			int tries = adjacentZone != -1 ? 1000 : 1000;
			bool firstZone = true;

			if (!rectangle)
			{
				while (size > 0 && tries > 0)
				{
					int x = r1.Next() % 16;
					int y = r1.Next() % 16;
					int minX = x, maxX = x, minY = y, maxY = y;
					if (!firstZone && zone[x, y] != zoneNumber)
					{
						continue;
					}
					if (firstZone)
					{
						if (zone[x, y] != 0) continue;

						if (adjacentZone != -1)
						{
							bool legalAdjaceny = false;

							if (x > 0 && zone[x - 1, y] == adjacentZone)
								legalAdjaceny = true;
							if (y > 0 && zone[x, y - 1] == adjacentZone)
								legalAdjaceny = true;
							if (x < 15 && zone[x + 1, y] == adjacentZone)
								legalAdjaceny = true;
							if (y < 15 && zone[x, y + 1] == adjacentZone)
								legalAdjaceny = true;

							if (legalAdjaceny)
								firstZone = false;
							else
								continue;
						}
						firstZone = false;
						zone[x, y] = zoneNumber;
					}

					tries--;
					int direction = r1.Next() % 16;
					int totalDirections = 0;
					if (direction % 16 >= 8) totalDirections++;
					if (direction % 8 >= 4) totalDirections++;
					if (direction % 4 >= 2) totalDirections++;
					if (direction % 2 >= 1) totalDirections++;
					if (totalDirections > size) continue;

					// 1 = north, 2 = east, 4 = south, 8 = west
					if (direction % 16 >= 8 && x != 0 && zone[x - 1, y] == 0 && (minX <= (x - 1) || maxX - minX <= 11))
					{
						zone[x - 1, y] = zoneNumber;
						minX = (x - 1 < minX ? x - 1 : minX);
						size--;
						tries = 100;
					}
					if (direction % 8 >= 4 && y != 15 && zone[x, y + 1] == 0 && (maxY >= (y + 1) || maxY - minY <= 11))
					{
						zone[x, y + 1] = zoneNumber;
						maxY = (y + 1 > maxY ? y + 1 : maxY);
						size--;
						tries = 100;
					}
					if (direction % 4 >= 2 && x != 15 && zone[x + 1, y] == 0 && (minX >= (x + 1) || maxX - minX <= 11))
					{
						zone[x + 1, y] = zoneNumber;
						maxX = (x + 1 > maxX ? x + 1 : maxX);
						size--;
						tries = 100;
					}
					if (direction % 2 >= 1 && y != 0 && zone[x, y - 1] == 0 && (minY <= (y - 1) || maxY - minY <= 11))
					{
						zone[x, y - 1] = zoneNumber;
						minY = (y - 1 < minY ? y - 1 : minY);
						size--;
						tries = 100;
					}
				}
				return (size <= 0);
			}
			else
			{
				int minMeasurement = (int)Math.Ceiling((double)size / 12);
				int maxMeasurement = (int)Math.Ceiling((double)size / minMeasurement);

				int length = ((r1.Next() % (maxMeasurement - minMeasurement)) + minMeasurement);
				int width = size / length;

				int x = (r1.Next() % (16 - length));
				int y = (r1.Next() % (16 - width));

				for (int i = x; i < x + length; i++)
					for (int j = y; j < y + width; j++)
						zone[i, j] = zoneNumber;

				return true;
			}
		}

		private bool reachable(int startY, int startX, bool water, int finishX, int finishY, int maxLake, bool alefgard)
		{
			int x = startX;
			int y = startY;

			List<int> validPlots = new List<int> { 1, 2, 3, 4, 5, 7, 0xe8, 0xe9, 0xeb, 0xec, 0xed, 0xee, 0xef, 0xf0, 0xf1, 0xf2, 0xf3, 0xf6 };
			if (water) validPlots.Add(0);

			bool first = true;
			List<int> toPlot = new List<int>();

			if (alefgard)
			{
				bool[,] plotted = new bool[139, 158];

				while (first || toPlot.Count != 0)
				{
					if (!first)
					{
						y = toPlot[0];
						toPlot.RemoveAt(0);
						x = toPlot[0];
						toPlot.RemoveAt(0);
					}
					else
					{
						first = false;
					}

					for (int dir = 0; dir < 5; dir++)
					{
						int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
						dirX = (dirX == 158 ? 0 : dirX == -1 ? 157 : dirX);
						int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
						dirY = (dirY == 139 ? 0 : dirY == -1 ? 138 : dirY);

						if (validPlots.Contains(map2[dirY, dirX]) && (map2[dirY, dirX] != 0 || island2[dirY, dirX] == maxLake))
						{
							if (dir != 0 && plotted[dirY, dirX] == false)
							{
								if (finishX == dirX && finishY == dirY)
									return true;
								toPlot.Add(dirY);
								toPlot.Add(dirX);
								plotted[dirY, dirX] = true;
							}
						}
					}
				}

				return false;
			}
			else
			{
				bool[,] plotted = new bool[256, 256];

				while (first || toPlot.Count != 0)
				{
					if (!first)
					{
						y = toPlot[0];
						toPlot.RemoveAt(0);
						x = toPlot[0];
						toPlot.RemoveAt(0);
					}
					else
					{
						first = false;
					}

					for (int dir = 0; dir < 5; dir++)
					{
						int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
						dirX = (dirX == 256 ? 0 : dirX == -1 ? 255 : dirX);
						int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
						dirY = (dirY == 256 ? 0 : dirY == -1 ? 255 : dirY);

						if (validPlots.Contains(map[dirY, dirX]) && (map[dirY, dirX] != 0 || island[dirY, dirX] == maxLake))
						{
							if (dir != 0 && plotted[dirY, dirX] == false)
							{
								if (finishX == dirX && finishY == dirY)
									return true;
								toPlot.Add(dirY);
								toPlot.Add(dirX);
								plotted[dirY, dirX] = true;
							}
						}
					}
				}

				return false;
			}
		}

		private int landPlot(int landNumber, int y, int x, int zoneToUse = 0)
		{
			bool first = true;
			List<int> toPlot = new List<int>();
			int plots = 1;
			while (first || toPlot.Count != 0)
			{
				if (!first)
				{
					y = toPlot[0];
					toPlot.RemoveAt(0);
					x = toPlot[0];
					toPlot.RemoveAt(0);
				}
				else
				{
					first = false;
				}

				for (int dir = 0; dir < 5; dir++)
				{
					if (zoneToUse != -1000)
					{
						int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
						dirX = (dirX == 256 ? 0 : dirX == -1 ? 255 : dirX);
						int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
						dirY = (dirY == 256 ? 0 : dirY == -1 ? 255 : dirY);

						if (island[dirY, dirX] == zoneToUse)
						{
							plots++;
							island[dirY, dirX] = landNumber;

							if (dir != 0)
							{
								toPlot.Add(dirY);
								toPlot.Add(dirX);
							}
						}
					}
					else
					{
						int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
						dirX = (dirX == 158 ? 0 : dirX == -1 ? 157 : dirX);
						int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
						dirY = (dirY == 138 ? 0 : dirY == -1 ? 137 : dirY);

						if (island2[dirY, dirX] == 0)
						{
							plots++;
							island2[dirY, dirX] = landNumber;

							if (dir != 0)
							{
								toPlot.Add(dirY);
								toPlot.Add(dirX);
							}
						}
					}
				}
			}

			return plots;
		}

		private bool legalPlacement(int y, int x)
		{
			for (int i = 60; i <= 74; i++)
			{
				int byteToUse = 0x3be1c + (i * 3);
				if (x == romData[byteToUse] && y == romData[byteToUse + 1])
					return false;
			}
			return true;
		}

		private bool validPlot(int y, int x, int height, int width, int[] legalIsland)
		{

			for (int lnI = 0; lnI < height; lnI++)
				for (int lnJ = 0; lnJ < width; lnJ++)
				{
					if (y + lnI >= (oSmallMap ? 128 : 256) || x + lnJ >= (oSmallMap ? 128 : 256)) return false;

					int legalY = (y + lnI >= 256 ? y - 256 + lnI : y + lnI);
					int legalX = (x + lnJ >= 256 ? x - 256 + lnJ : x + lnJ);

					bool ok = false;
					for (int lnK = 0; lnK < legalIsland.Length; lnK++)
						if (island[legalY, legalX] == legalIsland[lnK])
							ok = true;
					if (!ok) return false;
					if (map[legalY, legalX] == 0x00 || map[legalY, legalX] == 0x06 || map[legalY, legalX] >= 0xe8) // LAST CONDITION:  Castles, towns, villages, etc
						return false;
					// Prevent locations from spawning at the same place as locations on the other two maps.
					for (int lnK = 0; lnK < 11; lnK++)
					{
						int byteToUse = 0x3becd + (3 * lnI);
						if (legalX == romData[byteToUse] || legalY == romData[byteToUse + 1])
							return false;
					}

				}

			for (int lnI = -2; lnI <= 2; lnI++)
				for (int lnJ = -2; lnJ <= 2; lnJ++)
				{
					if (y + lnI >= (oSmallMap ? 128 : 256) || x + lnJ >= (oSmallMap ? 128 : 256)) return false;

					int legalY = (y + lnI >= 256 ? y - 256 + lnI : y + lnI);
					int legalX = (x + lnJ >= 256 ? x - 256 + lnJ : x + lnJ);

					// Make sure that the location isn't near a tower... to avoid falling... cascading into an entrance... which cascades into a hardlock.
					if (map[legalY, legalX] == 0xed || map[legalY, legalX] == 0xf1)
						return false;
				}

			return true;
		}

		private bool towerCheck(int y, int x)
		{
			if (Math.Abs(romData[0x23e86] - x) <= 2 && Math.Abs(romData[0x23e87] - y) <= 2)
				return false;

			return true;
		}

		private int lakePlot(int lakeNumber, int y, int x, bool fill = false, int islandNumber = -1)
		{
			bool first = true;
			List<int> toPlot = new List<int>();
			int plots = 1;
			//if (islandNumber >= 0) plots = 1;
			while (first || toPlot.Count != 0)
			{
				if (!first)
				{
					y = toPlot[0];
					toPlot.RemoveAt(0);
					x = toPlot[0];
					toPlot.RemoveAt(0);
				}
				else
				{
					if (fill)
						map[y, x] = (islandNumber == 0 ? 0x02 : islandNumber == 1 ? 0x03 : islandNumber == 2 ? 0x04 : islandNumber == 3 ? 0x01 : 0x05);
					first = false;
				}

				for (int dir = 0; dir < 5; dir++)
				{
					int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
					dirX = (dirX == 256 ? 0 : dirX == -1 ? 255 : dirX);
					int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
					dirY = (dirY == 256 ? 0 : dirY == -1 ? 255 : dirY);

					if (island[dirY, dirX] == -1 || (island[dirY, dirX] == lakeNumber && fill))
					{
						plots++;
						island[dirY, dirX] = (fill ? islandNumber : lakeNumber);
						if (fill)
							map[dirY, dirX] = (islandNumber == 0 ? 0x02 : islandNumber == 1 ? 0x03 : islandNumber == 2 ? 0x04 : islandNumber == 3 ? 0x01 : 0x05);

						if (dir != 0)
						{
							toPlot.Add(dirY);
							toPlot.Add(dirX);
						}
						//plots += lakePlot(lakeNumber, y, x, fill);
					}
				}
			}

			return plots;
		}

		private int lakePlot2(int lakeNumber, int y, int x, bool fill = false, int islandNumber = -1)
		{
			bool first = true;
			List<int> toPlot = new List<int>();
			int plots = 1;
			//if (islandNumber >= 0) plots = 1;
			while (first || toPlot.Count != 0)
			{
				if (!first)
				{
					y = toPlot[0];
					toPlot.RemoveAt(0);
					x = toPlot[0];
					toPlot.RemoveAt(0);
				}
				else
				{
					if (fill)
						map2[y, x] = (islandNumber == 0 ? 0x02 : islandNumber == 1 ? 0x03 : islandNumber == 2 ? 0x04 : islandNumber == 3 ? 0x01 : 0x05);
					first = false;
				}

				for (int dir = 0; dir < 5; dir++)
				{
					int dirX = (dir == 4 ? x - 1 : dir == 2 ? x + 1 : x);
					dirX = (dirX == 158 ? 0 : dirX == -1 ? 157 : dirX);
					int dirY = (dir == 1 ? y - 1 : dir == 3 ? y + 1 : y);
					dirY = (dirY == 139 ? 0 : dirY == -1 ? 138 : dirY);

					if (island2[dirY, dirX] == -1 || (island2[dirY, dirX] == lakeNumber && fill))
					{
						plots++;
						island2[dirY, dirX] = (fill ? islandNumber : lakeNumber);
						if (fill)
							map2[dirY, dirX] = (islandNumber == 0 ? 0x02 : islandNumber == 1 ? 0x03 : islandNumber == 2 ? 0x04 : islandNumber == 3 ? 0x01 : 0x05);

						if (dir != 0)
						{
							toPlot.Add(dirY);
							toPlot.Add(dirX);
						}
						//plots += lakePlot(lakeNumber, y, x, fill);
					}
				}
			}

			return plots;
		}

		private void setReturnPlacement(int x, int y, int locationIndex, bool placeShip = false, int maxLake = -1)
		{
			int byteToUseReturn = 0x7af10 + (7 * locationIndex);
			romData[byteToUseReturn + 1] = (byte)x;

			if (map[y + 1, x] == 0x00 || map[y + 1, x] == 0x06 || map[y + 1, x] > 0xe0)
				romData[byteToUseReturn + 2] = (byte)y;
			else
				romData[byteToUseReturn + 2] = (byte)(y + 1);

			if (placeShip)
				shipPlacement(byteToUseReturn + 3, y, x + 1, maxLake);

			// Balloony Placement
			if (map[y, x + 1] == 0x00 || map[y, x + 1] == 0x06)
				romData[byteToUseReturn + 5] = (byte)(x - 1);
			else
				romData[byteToUseReturn + 5] = (byte)(x + 1);
			romData[byteToUseReturn + 6] = (byte)y;
		}

		private void shipPlacement(int byteToUse, int top, int left, int maxLake = 0)
		{
			int minDirection = -99;
			int minDistance = 999;
			int finalX = 0;
			int finalY = 0;
			int distance = 0;
			int lnJ = top;
			int lnK = left;
			for (int lnI = 0; lnI < 4; lnI++)
			{
				lnJ = top;
				lnK = left;
				if (lnI == 0)
				{
					while (island[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnJ = (lnJ == 0 ? 255 : lnJ - 1);
					}
				}
				else if (lnI == 1)
				{
					while (island[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnJ = (lnJ == 255 ? 0 : lnJ + 1);
					}
				}
				else if (lnI == 2)
				{
					while (island[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnK = (lnK == 255 ? 0 : lnK + 1);
					}
				}
				else
				{
					while (island[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnK = (lnK == 0 ? 255 : lnK - 1);
					}
				}
				if (distance < minDistance)
				{
					minDistance = distance;
					minDirection = lnI;
					finalX = lnK;
					finalY = lnJ;
				}
				distance = 0;
			}
			romData[byteToUse] = (byte)(finalX);
			romData[byteToUse + 1] = (byte)(finalY);
			if (minDirection == 0)
			{
				lnJ = (finalY == 255 ? 0 : finalY + 1);
				while (map[lnJ, finalX] == 0x06)
				{
					map[lnJ, finalX] = 0x05;
					lnJ = (lnJ == 255 ? 0 : lnJ + 1);
				}
			}
			else if (minDirection == 1)
			{
				lnJ = (finalY == 0 ? 255 : finalY - 1);
				while (map[lnJ, finalX] == 0x06)
				{
					map[lnJ, finalX] = 0x05;
					lnJ = (lnJ == 0 ? 255 : lnJ - 1);
				}
			}
			else if (minDirection == 2)
			{
				lnK = (finalX == 0 ? 255 : finalX - 1);
				while (map[finalY, lnK] == 0x06)
				{
					map[finalY, lnK] = 0x05;
					lnK = (lnK == 0 ? 255 : lnK - 1);
				}
			}
			else
			{
				lnK = (finalX == 255 ? 0 : finalX + 1);
				while (map[finalY, lnK] == 0x06)
				{
					map[finalY, lnK] = 0x05;
					lnK = (lnK == 255 ? 0 : lnK + 1);
				}
			}
		}

		private void shipPlacement2(int byteToUse, int top, int left, int maxLake = 0)
		{
			int minDirection = -99;
			int minDistance = 999;
			int finalX = 0;
			int finalY = 0;
			int distance = 0;
			int lnJ = top;
			int lnK = left;
			for (int lnI = 0; lnI < 4; lnI++)
			{
				lnJ = top;
				lnK = left;
				if (lnI == 0)
				{
					while (island2[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnJ = (lnJ == 0 ? 138 : lnJ - 1);
					}
				}
				else if (lnI == 1)
				{
					while (island2[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnJ = (lnJ == 138 ? 0 : lnJ + 1);
					}
				}
				else if (lnI == 2)
				{
					while (island2[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnK = (lnK == 157 ? 0 : lnK + 1);
					}
				}
				else
				{
					while (island2[lnJ, lnK] != maxLake && distance < 200)
					{
						distance++;
						lnK = (lnK == 0 ? 157 : lnK - 1);
					}
				}
				if (distance < minDistance)
				{
					minDistance = distance;
					minDirection = lnI;
					finalX = lnK;
					finalY = lnJ;
				}
				distance = 0;
			}
			romData[byteToUse] = (byte)(finalX);
			romData[byteToUse + 1] = (byte)(finalY);
			if (minDirection == 0)
			{
				lnJ = (finalY == 255 ? 0 : finalY + 1);
				while (map2[lnJ, finalX] == 0x06)
				{
					map2[lnJ, finalX] = 0x05;
					lnJ = (lnJ == 255 ? 0 : lnJ + 1);
				}
			}
			else if (minDirection == 1)
			{
				lnJ = (finalY == 0 ? 255 : finalY - 1);
				while (map2[lnJ, finalX] == 0x06)
				{
					map2[lnJ, finalX] = 0x05;
					lnJ = (lnJ == 0 ? 255 : lnJ - 1);
				}
			}
			else if (minDirection == 2)
			{
				lnK = (finalX == 0 ? 255 : finalX - 1);
				while (map2[finalY, lnK] == 0x06)
				{
					map2[finalY, lnK] = 0x05;
					lnK = (lnK == 0 ? 255 : lnK - 1);
				}
			}
			else
			{
				lnK = (finalX == 255 ? 0 : finalX + 1);
				while (map2[finalY, lnK] == 0x06)
				{
					map2[finalY, lnK] = 0x05;
					lnK = (lnK == 255 ? 0 : lnK + 1);
				}
			}
		}

		private void connectLikeIslands(Random r1)
		{
			for (int lnI = 1; lnI <= 10; lnI++)
			{
				//if (lnI == 9) lnI = lnI;
				while (1 == 1)
				{
					List<SuperIslandLinks> links1 = new List<SuperIslandLinks>();
					List<SuperIslandLinks> links2 = new List<SuperIslandLinks>();

					int id1 = 0;
					int id2 = 0;

					// Take the max island and get the closest link to an island that's in the same zone
					for (int x = 0; x < 256; x++)
						for (int y = 0; y < 256; y++)
						{
							if (island[y, x] == maxIsland[lnI] && map[y, x] != 0x00 && map[y - 1, x] != 0x06)
							{
								id1++;
								links1.Add(new SuperIslandLinks { island = island[y, x], id = id1, x = x, y = y });
							}
							if (island[y, x] != maxIsland[lnI] && island[y, x] / 1000 == maxIsland[lnI] / 1000 && map[y, x] != 0x00 && map[y - 1, x] != 0x06)
							{
								id2++;
								links2.Add(new SuperIslandLinks { island = island[y, x], id = id2, x = x, y = y });
							}
						}

					if (id2 == 0)
						break;

					int minDistance = 9999;
					int finalX1 = -1;
					int finalX2 = -1;
					int finalY1 = -1;
					int finalY2 = -1;
					int oldIsland = -1;

					foreach (SuperIslandLinks link in links1)
						foreach (SuperIslandLinks link2 in links2)
						{
							if (Math.Abs(link.x - link2.x) + Math.Abs(link.y - link2.y) < minDistance)
							{
								// Disqualify a link if it would go through a different zone
								bool disqualified = false;
								int segment = (oSmallMap ? 8 : 16);

								if (link.y != link2.y)
								{
									if (link.y < link2.y)
									{
										for (int lnJ = link.y + 1; lnJ <= link2.y; lnJ++)
											if (zone[lnJ / segment, link.x / segment] / 1000 != lnI)
												disqualified = true;
									}
									else
									{
										for (int lnJ = link.y - 1; lnJ >= link2.y; lnJ--)
											if (zone[lnJ / segment, link.x / segment] / 1000 != lnI)
												disqualified = true;
									}
								}

								if (link.x != link2.x)
								{
									if (link.x < link2.x)
									{
										for (int lnJ = link.x + 1; lnJ <= link2.x; lnJ++)
											if (zone[link2.y / segment, lnJ / segment] / 1000 != lnI)
												disqualified = true;
									}
									else
									{
										for (int lnJ = link.x - 1; lnJ >= link2.x; lnJ--)
											if (zone[link2.y / segment, lnJ / segment] / 1000 != lnI)
												disqualified = true;
									}
								}

								if (!disqualified)
								{
									finalX1 = link.x;
									finalX2 = link2.x;
									finalY1 = link.y;
									finalY2 = link2.y;
									oldIsland = link2.island;

									minDistance = Math.Abs(link.x - link2.x) + Math.Abs(link.y - link2.y);
								}
							}
						}

					if (oldIsland == -1)
						break;

					byte randomLand = (byte)((r1.Next() % 5) + 1);

					if (finalY1 != finalY2)
					{
						if (finalY1 < finalY2)
						{
							for (int lnJ = finalY1 + 1; lnJ <= finalY2; lnJ++)
							{
								map[lnJ, finalX1] = randomLand;
								//island[lnJ, finalX1] = maxIsland[lnI];
							}
						}
						else
						{
							for (int lnJ = finalY1 - 1; lnJ >= finalY2; lnJ--)
							{
								map[lnJ, finalX1] = randomLand;
								//island[lnJ, finalX1] = maxIsland[lnI];
							}
						}
					}

					if (finalX1 != finalX2)
					{
						if (finalX1 < finalX2)
						{
							for (int lnJ = finalX1 + 1; lnJ <= finalX2; lnJ++)
							{
								map[finalY2, lnJ] = randomLand;
								//island[finalY2, lnJ] = maxIsland[lnI];
							}
						}
						else
						{
							for (int lnJ = finalX1 - 1; lnJ >= finalX2; lnJ--)
							{
								map[finalY2, lnJ] = randomLand;
								//island[finalY2, lnJ] = maxIsland[lnI];
							}
						}
					}

					List<SuperIslandLinks> islandConverts = links2.Where(c => c.island == oldIsland).ToList();
					foreach (SuperIslandLinks singleConvert in islandConverts)
					{
						island[singleConvert.y, singleConvert.x] = maxIsland[lnI];
					}
				}
			}
		}

		private bool connectIslands(Random r1, int island1, int island2, bool mountains)
		{
			// First connect like-islands together until no more links can be made.
			try
			{
				List<SuperIslandLinks> links1 = new List<SuperIslandLinks>();
				List<SuperIslandLinks> links2 = new List<SuperIslandLinks>();

				int id1 = 0;
				int id2 = 0;

				for (int x = 0; x < 256; x++)
					for (int y = 0; y < 256; y++)
					{
						if (island[y, x] == maxIsland[island1] && map[y, x] != 0x00 && (island1 == 5 ? map[y, x - 1] != 0x06 : map[y - 1, x] != 0x06) &&
							(map[y - 1, x] == 0x00 || map[y - 1, x] == 0x06 ||
							map[y, x - 1] == 0x00 || map[y, x - 1] == 0x06 ||
							map[y + 1, x] == 0x00 || map[y + 1, x] == 0x06 ||
							map[y, x + 1] == 0x00 || map[y, x + 1] == 0x06))
						{
							id1++;
							links1.Add(new SuperIslandLinks { island = island1, id = id1, x = x, y = y });
						}
						if (island[y, x] == maxIsland[island2] && map[y, x] != 0x00 && (island1 == 5 ? map[y, x - 1] != 0x06 : map[y - 1, x] != 0x06) &&
							(map[y - 1, x] == 0x00 || map[y - 1, x] == 0x06 ||
							map[y, x - 1] == 0x00 || map[y, x - 1] == 0x06 ||
							map[y + 1, x] == 0x00 || map[y + 1, x] == 0x06 ||
							map[y, x + 1] == 0x00 || map[y, x + 1] == 0x06))
						{
							id2++;
							links2.Add(new SuperIslandLinks { island = island2, id = id2, x = x, y = y });
						}
					}

				int minDistance = 9999;
				int finalX1 = -1;
				int finalX2 = -1;
				int finalY1 = -1;
				int finalY2 = -1;

				foreach (SuperIslandLinks link in links1)
					foreach (SuperIslandLinks link2 in links2)
					{
						if (Math.Abs(link.x - link2.x) + Math.Abs(link.y - link2.y) < minDistance
						  && Math.Abs(link.y - link2.y) > 1)  // (island1 == 5 || Math.Abs(link.y - link2.y) > 1))
							//if (island1 == 3 || island1 == 9 || (island1 == 5 && Math.Abs(link.x - link2.x) > 1))
							{
								// Disqualify a link if it would go through a different zone
								bool disqualified = false;
								int segment = (oSmallMap ? 8 : 16);

								if (island1 == 5)
								{
									if (link.x != link2.x)
									{
										if (link.x < link2.x)
										{
											for (int lnJ = link.x + 1; lnJ <= link2.x; lnJ++)
												if (zone[link.y / segment, lnJ / segment] / 1000 != island1 && zone[link.y / segment, lnJ / segment] / 1000 != island2)
													disqualified = true;
										}
										else
										{
											for (int lnJ = link.x - 1; lnJ >= link2.x; lnJ--)
												if (zone[link.y / segment, lnJ / segment] / 1000 != island1 && zone[link.y / segment, lnJ / segment] / 1000 != island2)
													disqualified = true;
										}
									}

									if (link.y != link2.y)
									{
										if (link.y < link2.y)
										{
											for (int lnJ = link.y + 1; lnJ <= link2.y; lnJ++)
												if (zone[lnJ / segment, link2.x / segment] / 1000 != island1 && zone[lnJ / segment, link2.x / segment] / 1000 != island2)
													disqualified = true;
										}
										else
										{
											for (int lnJ = link.y - 1; lnJ >= link2.y; lnJ--)
												if (zone[lnJ / segment, link2.x / segment] / 1000 != island1 && zone[lnJ / segment, link2.x / segment] / 1000 != island2)
													disqualified = true;
										}
									}
								}
								else
								{
									if (link.y != link2.y)
									{
										if (link.y < link2.y)
										{
											for (int lnJ = link.y + 1; lnJ <= link2.y; lnJ++)
												if (zone[lnJ / segment, link.x / segment] / 1000 != island1 && zone[lnJ / segment, link.x / segment] / 1000 != island2)
													disqualified = true;
										}
										else
										{
											for (int lnJ = link.y - 1; lnJ >= link2.y; lnJ--)
												if (zone[lnJ / segment, link.x / segment] / 1000 != island1 && zone[lnJ / segment, link.x / segment] / 1000 != island2)
													disqualified = true;
										}
									}

									if (link.x != link2.x)
									{
										if (link.x < link2.x)
										{
											for (int lnJ = link.x + 1; lnJ <= link2.x; lnJ++)
												if (zone[link2.y / segment, lnJ / segment] / 1000 != island1 && zone[link2.y / segment, lnJ / segment] / 1000 != island2)
													disqualified = true;
										}
										else
										{
											for (int lnJ = link.x - 1; lnJ >= link2.x; lnJ--)
												if (zone[link2.y / segment, lnJ / segment] / 1000 != island1 && zone[link2.y / segment, lnJ / segment] / 1000 != island2)
													disqualified = true;
										}
									}
								}

								if (!disqualified)
								{
									finalX1 = link.x;
									finalX2 = link2.x;
									finalY1 = link.y;
									finalY2 = link2.y;

									minDistance = Math.Abs(link.x - link2.x) + Math.Abs(link.y - link2.y);
								}
							}
					}

				if (finalX1 == -1) return false;

				byte prevTile;

				// Now that it's established, we need to build the land bridge and possibly set town coordinates (island1 = 3, island2 = 4)
				// Go N/S, then E/W
				if (island1 == 3 || island1 == 9)
				{
					map[finalY1, finalX1] = island1 == 3 ? 0xf0 : 0xec;
					if (finalY1 < finalY2)
					{
						prevTile = (byte)(map[finalY1 - 1, finalX1 - 2] == 0x00 || map[finalY1 - 1, finalX1 - 2] == 0x06 ? 0x02 : map[finalY1 - 1, finalX1 - 2]);
						map[finalY1 - 1, finalX1] = map[finalY1 - 1, finalX1 - 1] = map[finalY1 - 1, finalX1 + 1] = prevTile;
						// Sometimes there's an awkward placement that blocks off the path to the location...
						if ((map[finalY1, finalX1 - 1] == 0x00 || map[finalY1, finalX1 - 2] == 0x00) && (map[finalY1, finalX1 + 1] == 0x00 || map[finalY1, finalX1 + 2] == 0x00))
						{
							if (map[finalY1 + 1, finalX1 + 2] != 0x00)
								map[finalY1, finalX1 + 2] = prevTile;
							else
								map[finalY1, finalX1 - 2] = prevTile;
						}
						island[finalY1 - 1, finalX1] = island[finalY1 - 1, finalX1 - 1] = island[finalY1 - 1, finalX1 + 1] = maxIsland[island1];
					}
					if (finalY1 > finalY2)
					{
						prevTile = (byte)(map[finalY1 + 1, finalX1 - 2] == 0x00 || map[finalY1 + 1, finalX1 - 2] == 0x06 ? 0x02 : map[finalY1 + 1, finalX1 - 2]);
						map[finalY1 + 1, finalX1] = map[finalY1 + 1, finalX1 - 1] = map[finalY1 + 1, finalX1 + 1] = prevTile;
						if ((map[finalY1, finalX1 - 1] == 0x00 || map[finalY1, finalX1 - 2] == 0x00) && (map[finalY1, finalX1 + 1] == 0x00 || map[finalY1, finalX1 + 2] == 0x00))
						{
							if (map[finalY1 - 1, finalX1 + 2] != 0x00)
								map[finalY1, finalX1 + 2] = prevTile;
							else
								map[finalY1, finalX1 - 2] = prevTile;
						}
						island[finalY1 + 1, finalX1] = island[finalY1 + 1, finalX1 - 1] = island[finalY1 + 1, finalX1 + 1] = maxIsland[island1];
					}

					if (island1 == 3)
					{
						int byteToUse = 0x3be1c + (3 * 14);
						romData[byteToUse] = (byte)finalX1;
						romData[byteToUse + 1] = (byte)finalY1;

						// Need to replace X and Y to new Tempe Location
						romData[0x72ee1] = (byte)finalX1;
						romData[0x72ee5] = (byte)finalY1;

						setReturnPlacement(finalX1, finalY1 < finalY2 ? finalY1 - 1 : finalY1 + 1, 15);
					}
					else if (island1 == 9)
					{
						int byteToUse = 0x3be1c + (3 * 43);
						romData[byteToUse] = (byte)finalX1;
						romData[byteToUse + 1] = (byte)finalY1;
					}

					if (finalY1 < finalY2 && island1 == 3)
					{
						romData[0x230b5] = 0xe6; // Increment instead of decrement
						romData[0x230d8] = 0xc6; // Decrement instead of increment
						romData[0x22f80] = 0x00; // Need to move up instead of down
					}
					if (finalY1 > finalY2 && island1 == 9)
					{
						romData[0x23134] = 0xe6; // Increment instead of decrement
						romData[0x2313d] = 0xc6; // Decrement instead of increment
					}
				}

				byte randomLand = (byte)((r1.Next() % 5) + 1);

				//if (island1 == 5)
				//{
				//	connectXIslands(finalX1, finalX2, finalY1, finalY2, island1, randomLand);
				//	connectYIslands(finalX1, finalX2, finalY1, finalY2, island1, island2, mountains, randomLand);
				//}
				//else
				//{
				connectYIslands(finalX1, finalX2, finalY1, finalY2, island1, island2, mountains, randomLand);
				connectXIslands(finalX1, finalX2, finalY1, finalY2, island1, island2, randomLand);
				//}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}


			return true;
		}

		private void connectXIslands(int finalX1, int finalX2, int finalY1, int finalY2, int island1, int island2, int randomLand)
		{
			if (finalX1 < finalX2)
			{
				if (finalY1 != finalY2 && island1 != 5)
					map[finalY2, finalX1 - 1] = 0x00;
				//island[finalY2, finalX1 - 1] = -999;

				for (int lnI = finalX1 + 1; lnI <= finalX2; lnI++)
				{
					map[finalY2, lnI] = randomLand; // island1 == 5 ? finalY1 : 
													//island[lnI, finalX1] = (island1 == 5 ? maxIsland[island1] : maxIsland[island2]);

					//if (lnI == finalX2 - 1 && island1 == 5)
					//{
					//	map[finalY1, lnI] = 0xf6;
					//	map[finalY1 - 1, lnI] = 0x00;
					//	map[finalY1 + 1, lnI] = 0x00;
					//}
					//if (island1 == 5 && lnI != finalX2) //  && lnI != finalX2
					//{
					//	map[finalY1 - 1, lnI] = 0x00;
					//	map[finalY1 + 1, lnI] = 0x00;
					//	//if (finalY1 > finalY2)
					//	//	map[finalY1 - 2, lnI + 1] = randomLand;
					//	//else
					//	//	map[finalY1 + 2, lnI + 1] = randomLand;
					//}
				}

				//if (finalY1 != finalY2 && island1 == 5) { map[finalY1, finalX2 + 1] = 0x00; }
			}
			else if (finalX1 > finalX2)
			{
				if (finalY1 != finalY2 && island1 != 5)
					map[finalY2, finalX1 + 1] = 0x00;
				//island[finalY2, finalX1 + 1] = -999;

				for (int lnI = finalX1 - 1; lnI >= finalX2; lnI--)
				{
					map[finalY2, lnI] = randomLand; // island1 == 5 ? finalY1 : 
													//island[lnI, finalX1] = (island1 == 5 ? maxIsland[island1] : maxIsland[island2]);

					//if (lnI == finalX2 + 1 && island1 == 5)
					//{
					//	map[finalY1, lnI] = 0xf6;
					//	map[finalY1 - 1, lnI] = 0x00;
					//	map[finalY1 + 1, lnI] = 0x00;
					//}
					//if (island1 == 5 && lnI != finalX2) //  && lnI != finalX2
					//{
						//map[finalY1 - 1, lnI] = 0x00;
						//map[finalY1 + 1, lnI] = 0x00;
						//if (finalY1 > finalY2)
						//	map[finalY1 - 2, lnI - 1] = randomLand;
						//else
						//	map[finalY1 + 2, lnI - 1] = randomLand;
					//}
				}

				//if (finalY1 != finalY2 && island1 == 5) { map[finalY1, finalX2 - 1] = 0x00; }
			}

			// Create a bridge if necessary
			if (island1 == 5)
			{
				if (finalX1 == finalX2)
				{
					// If the connecting island was south of the originating island...
					if (finalY2 > finalY1)
					{
						// Place bridges on both sides...
						if (island[finalY2, finalX2 + 3] != maxIsland[5])
						{
							map[finalY2, finalX2 + 1] = 0xf6;
							for (int iy = 0; iy <= 2; iy++)
								map[finalY2 + iy, finalX2 + 2] = randomLand;
						}
						if (island[finalY2, finalX2 - 3] != maxIsland[5])
						{
							map[finalY2, finalX2 - 1] = 0xf6;
							for (int iy = 0; iy <= 2; iy++)
								map[finalY2 + iy, finalX2 - 2] = randomLand;
						}
						map[finalY2 + 1, finalX2 - 1] = map[finalY2 + 1, finalX2] = map[finalY2 + 1, finalX2 + 1] = 0x00;
						map[finalY2 + 2, finalX2 - 1] = map[finalY2 + 2, finalX2] = map[finalY2 + 2, finalX2 + 1] = randomLand;
					}
					else if (finalY2 < finalY1)
					{
						if (island[finalY2, finalX2 + 3] != maxIsland[5])
						{
							map[finalY2, finalX2 + 1] = 0xf6;
							for (int iy = 0; iy <= 2; iy++)
								map[finalY2 - iy, finalX2 + 2] = randomLand;
						}
						if (island[finalY2, finalX2 - 3] != maxIsland[5])
						{
							map[finalY2, finalX2 - 1] = 0xf6;
							for (int iy = 0; iy <= 2; iy++)
								map[finalY2 - iy, finalX2 - 2] = randomLand;
						}
						map[finalY2 - 1, finalX2 - 1] = map[finalY2 - 1, finalX2] = map[finalY2 - 1, finalX2 + 1] = 0x00;
						map[finalY2 - 2, finalX2 - 1] = map[finalY2 - 2, finalX2] = map[finalY2 - 2, finalX2 + 1] = randomLand;
					}
				} else if (finalX1 < finalX2)
				{
					map[finalY2, finalX2] = 0xf6;
					map[finalY2 - 1, finalX2] = 0x00;
					map[finalY2 + 1, finalX2] = 0x00;
					if (finalX2 - finalX1 >= 2)
					{
						map[finalY2 - 1, finalX2 - 1] = 0x00;
						map[finalY2 + 1, finalX2 - 1] = 0x00;
					}
				} else // finalX1 > finalX2
				{
					map[finalY2, finalX2] = 0xf6;
					map[finalY2 - 1, finalX2] = 0x00;
					map[finalY2 + 1, finalX2] = 0x00;
					if (finalX1 - finalX2 >= 2)
					{
						map[finalY2 - 1, finalX2 + 1] = 0x00;
						map[finalY2 + 1, finalX2 + 1] = 0x00;
					}
				}
			}
		}

		private void connectYIslands(int finalX1, int finalX2, int finalY1, int finalY2, int island1, int island2, bool mountains, int randomLand)
		{
			// If island1 is north of island2...
			if (finalY1 < finalY2)
			{
				for (int lnI = finalY1 + 1; lnI <= finalY2; lnI++)
				{
					map[lnI, finalX1] = randomLand; // island1 == 5 ? finalX2 : 
													//island[lnI, finalX1] = (island1 == 5 ? maxIsland[island1] : maxIsland[island2]);

					if (mountains && lnI != finalY2 && map[lnI, finalX1 - 1] != 0x00 && island[lnI, finalX1 - 1] / 1000 != island2) // island1 == 5 ? finalX2 - 1 : 
						map[lnI, finalX1 - 1] = 0x06; // island1 == 5 ? finalX2 - 1 : 
					if (mountains && lnI != finalY2 && map[lnI, finalX1 + 1] != 0x00 && island[lnI, finalX1 + 1] / 1000 != island2) // island1 == 5 ? finalX2 + 1 : 
						map[lnI, finalX1 + 1] = 0x06; // island1 == 5 ? finalX2 + 1 : 
				}
				if (finalX1 != finalX2) { map[finalY2 + 1, finalX1] = 0x00; } //  && island1 != 5
			}
			else
			{
				for (int lnI = finalY1 - 1; lnI >= finalY2; lnI--)
				{
					map[lnI, finalX1] = randomLand; // island1 == 5 ? finalX2 : 
													//island[lnI, finalX1] = (island1 == 5 ? maxIsland[island1] : maxIsland[island2]);

					if (mountains && lnI != finalY2 && map[lnI, finalX1 - 1] != 0x00 && island[lnI, finalX1 - 1] / 1000 != island2) // island1 == 5 ? finalX2 - 1 : 
						map[lnI, finalX1 - 1] = 0x06; // island1 == 5 ? finalX2 - 1 : 
					if (mountains && lnI != finalY2 && map[lnI, finalX1 + 1] != 0x00 && island[lnI, finalX1 + 1] / 1000 != island2) // island1 == 5 ? finalX2 + 1 : 
						map[lnI, finalX1 + 1] = 0x06; // island1 == 5 ? finalX2 + 1 : 
				}
				if (finalX1 != finalX2) { map[finalY2 - 1, finalX1] = 0x00; } //  && island1 != 5
			}
		}
	}
}