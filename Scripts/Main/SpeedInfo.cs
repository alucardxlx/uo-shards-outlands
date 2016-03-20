using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server
{
    public class SpeedInfo
    {
        private static bool Enabled = true;

        private double m_ActiveSpeed;
        private double m_PassiveSpeed;
        private Type[] m_Types;

        public double ActiveSpeed
        {
            get { return m_ActiveSpeed; }
            set { m_ActiveSpeed = value; }
        }

        public double PassiveSpeed
        {
            get { return m_PassiveSpeed; }
            set { m_PassiveSpeed = value; }
        }

        public Type[] Types
        {
            get { return m_Types; }
            set { m_Types = value; }
        }

        public SpeedInfo(double activeSpeed, double passiveSpeed, Type[] types)
        {
            m_ActiveSpeed = activeSpeed;
            m_PassiveSpeed = passiveSpeed;
            m_Types = types;
        }

        public static bool Contains(object obj)
        {
            if (!Enabled)
                return false;

            if (m_Table == null)
                LoadTable();

            SpeedInfo sp = (SpeedInfo)m_Table[obj.GetType()];

            return (sp != null);
        }

        public static bool GetSpeeds(object obj, ref double activeSpeed, ref double passiveSpeed)
        {
            if (!Enabled)
                return false;

            if (m_Table == null)
                LoadTable();

            SpeedInfo sp = (SpeedInfo)m_Table[obj.GetType()];

            if (sp == null)
            {
                activeSpeed = 0.5;
                passiveSpeed = 0.6;

                return false;
            }

            activeSpeed = sp.ActiveSpeed;
            passiveSpeed = sp.PassiveSpeed;

            return true;
        }

        private static void LoadTable()
        {
            m_Table = new Hashtable();

            for (int i = 0; i < m_Speeds.Length; ++i)
            {
                SpeedInfo info = m_Speeds[i];
                Type[] types = info.Types;

                for (int j = 0; j < types.Length; ++j)
                    m_Table[types[j]] = info;
            }
        }

        private static Hashtable m_Table;

        private static SpeedInfo[] m_Speeds = new SpeedInfo[]
            {  
                //Ultra Slow
				new SpeedInfo( 0.8, 1.0, new Type[]
                {
                    typeof( TheCreeper ),
                } ),

                //Very Slow
				new SpeedInfo( 0.6, 0.7, new Type[]
                {
                    typeof( Zombie ),
                    typeof( FlamingZombie ),
                    typeof( BloodyZombie ),
                    typeof( DecayedZombie ),
                    typeof( Mummy ),
                    typeof( RottingCorpse ),
                    typeof( EvilOak ),
                    typeof( Reaper ),
                    typeof( FrostwoodReaper ),
                    typeof( GreaterSlime ),
                    typeof( PoisonousSlime ),
                    typeof( CorrosiveSlime ),
                    typeof( Slime ),
                    typeof( FrostOoze ),
                    typeof( Jwilson ),
                    typeof( AncientSlime ),
                    typeof( VoidSlime ),
                    typeof( SlimeTroll ),
                    typeof( ZombieMagi ),
                    typeof( Myconid ),
                    typeof( MyconidTallstalk ),
                    typeof( NightbarkTree ),
                    typeof( DeDOSBotNetZombie ),
                    typeof( DeDOS ),
                    typeof( BloodSlime ),
                } ),

                //Slow
				new SpeedInfo( 0.5, 0.6, new Type[]
                {
                    typeof( AntLion ),
                    typeof( ArcticOgreLord ),
                    typeof( BogThing ),
                    typeof( Bogle ),
                    typeof( BoneKnight ),
                    typeof( EarthElemental ),
                    typeof( Ettin ),
                    typeof( FrostTroll ),
                    typeof( GazerLarva ),
                    typeof( Ghoul ),
                    typeof( Golem ),
                    typeof( HeadlessOne ),
                    typeof( Juggernaut ),
                    typeof( Ogre ),
                    typeof( OgreMage ),
                    typeof( OgreLord ),
                    typeof( PlagueBeast ),
                    typeof( Quagmire ),
                    typeof( Rat ),
                    typeof( Beetle ),
                    typeof( Sewerrat ),
                    typeof( Skeleton ),
                    typeof( Walrus ),
                    typeof( RestlessSoul ),
                    typeof( Serado ),
                    typeof( ValoriteElemental ),
                    typeof( VeriteElemental ),
                    typeof( LuniteElemental ),
                    typeof( Titan ),
                    typeof( Troll ),
                    typeof( AgapiteElemental ),
                    typeof( BronzeElemental ),
                    typeof( CopperElemental ),
                    typeof( DullCopperElemental ),
                    typeof( GoldenElemental ),
                    typeof( IceElemental ),
                    typeof( ElderIceElemental ),
                    typeof( PlagueSpawn ),
                    typeof( ShadowIronElemental ),
                    typeof( SnowElemental ),
                    typeof( ElderSnowElemental ),
                    typeof( CrystalElemental ),
                    typeof( DarknightCreeper ),
                    typeof( MoundOfMaggots ),
                    typeof( Yamandon ),
                    typeof( ArmoredCrab ),
                    typeof( Crab ),
                    typeof( DeepCrab ),
                    typeof( BurrowBeetle ),
                    typeof( EarthOreling ),
                    typeof( RockOreling ),
                    typeof( RockElemental ),
                    typeof( Guar ),
                    typeof( SwampCrawler ),
                    typeof( SkeletalDrake ),
                    typeof( Sphinx ),
                    typeof( MysterySphinx ),
                    typeof( Gauth ),
                    typeof( Beholder ),
                    typeof( Aboleth ),
                    typeof( Glowvines ),
                    typeof( RisenKnight ),
                    typeof( RisenNoble ),
                    typeof( RisenHonorGuard ),
                    typeof( RisenRoyal ),
                    typeof( BloodyReaver ),
                    typeof( IchibodShame ),                    
                    typeof( LordOfBones ),
                    typeof( ArborealMyconid ),
                    typeof( TreeStalker ),
                    typeof( Ent ),
                    typeof( TreeOfLife ),
                    typeof( OtherworldlyDenizon ),
                } ),

                //Medium
				new SpeedInfo( 0.4, 0.5, new Type[]
                {
                    typeof( EnergyVortex ),
                    typeof( BladeSpirits ),
                    typeof( ToxicElemental ),
                    typeof( ElderToxicElemental ),
                    typeof( AncientLich ),
                    typeof( MagusLich ),
                    typeof( BlackSolenInfiltratorQueen ),
                    typeof( BlackSolenInfiltratorWarrior ),
                    typeof( BlackSolenQueen ),
                    typeof( BlackSolenWarrior ),
                    typeof( BlackSolenWorker ),
                    typeof( BloodElemental ),
                    typeof( ElderBloodElemental ),
                    typeof( Boar ),
                    typeof( Bogling ),
                    typeof( BoneMagi ),
                    typeof( Bull ),
                    typeof( Bison ),
                    typeof( BullFrog ),
                    typeof( Centaur ),
                    typeof( ChaosDaemon ),
                    typeof( Chicken ),
                    typeof( GolemController ),
                    typeof( Cow ),
                    typeof( Cyclops ),
                    typeof( CyclopsShaman ),
                    typeof( GreaterCyclops ),
                    typeof( Daemon ),
                    typeof( DeepSeaSerpent ),
                    typeof( Drake ),
                    typeof( ArcaneDrake ),
                    typeof( ShadowDrake ),
                    typeof( ElderGazer ),
                    typeof( EvilMage ),
                    typeof( EvilMageLord ),
                    typeof( ElementalSeer ),
                    typeof( FlameElementalist ),
                    typeof( EnergyElementalist ),
                    typeof( CorruptRunecaster ),
                    typeof( CorruptWarmage ),
                    typeof( Executioner ),
                    typeof( CorruptReaver ),
                    typeof( FireElemental ),
                    typeof( ElderFireElemental ),
                    typeof( AncientFlame ),
                    typeof( FireGargoyle ),
                    typeof( FrostSpider ),
                    typeof( Gargoyle ),
                    typeof( Gazer ),
                    typeof( IceSerpent ),
                    typeof( GiantRat ),
                    typeof( PlagueRat ),
                    typeof( GiantSerpent ),
                    typeof( GiantSpider ),
                    typeof( GiantToad ),
                    typeof( Goat ),
                    typeof( Guardian ),
                    typeof( Harpy ),
                    typeof( Harrower ),
                    typeof( HordeMinion ),
                    typeof( IceFiend ),
                    typeof( IceFiendLord ),
                    typeof( IceSnake ),
                    typeof( Imp ),
                    typeof( JackRabbit ),
                    typeof( Kirin ),
                    typeof( Kraken ),
                    typeof( PredatorHellCat ),
                    typeof( LavaLizard ),
                    typeof( IceLizard ),
                    typeof( LavaSerpent ),
                    typeof( LavaSnake ),
                    typeof( Lizardman ),
                    typeof( Llama ),
                    typeof( Mongbat ),
                    typeof( MysteryMongbat ),
                    typeof( StrongMongbat ),
                    typeof( MountainGoat ),
                    typeof( Orc ),
                    typeof( OrcBomber ),
                    typeof( OrcBrute ),
                    typeof( OrcCaptain ),
                    typeof( OrcishLord ),
                    typeof( OrcishMage ),
                    typeof( FrostOrc ),
                    typeof( FrostOrcLord ),
                    typeof( FrostOrcMage ),
                    typeof( Pig ),
                    typeof( Ratman ),
                    typeof( RatmanArcher ),
                    typeof( RatmanMage ),
                    typeof( RedSolenInfiltratorQueen ),
                    typeof( RedSolenInfiltratorWarrior ),
                    typeof( RedSolenQueen ),
                    typeof( RedSolenWarrior ),
                    typeof( RedSolenWorker ),
                    typeof( Scorpion ),
                    typeof( ChromaticCrawler ),
                    typeof( SeaSerpent ),
                    typeof( SerpentineDragon ),
                    typeof( Shade ),
                    typeof( Basilisk ),
                    typeof( ShadowWisp ),
                    typeof( ShadowWyrm ),
                    typeof( Sheep ),
                    typeof( SkeletalDragon ),
                    typeof( SkeletalMage ),
                    typeof( Snake ),
                    typeof( SpectralArmour ),
                    typeof( Spectre ),
                    typeof( StoneGargoyle ),
                    typeof( StoneHarpy ),
                    typeof( SwampDragon ),
                    typeof( ScaledSwampDragon ),
                    typeof( EtherealSwampDragon ),
                    typeof( SwampTentacle ),
                    typeof( TerathanAvenger ),
                    typeof( TerathanDrone ),
                    typeof( TerathanMatriarch ),
                    typeof( TerathanWarrior ),
                    typeof( WaterElemental ),
                    typeof( ElderWaterElemental ),
                    typeof( ElderAirElemental ),
                    typeof( FountainOfEvil ),
                    typeof( Puddle ),
                    typeof( DeepWater ),
                    typeof( WhippingVine ),
                    typeof( Wraith ),
                    typeof( Wyvern ),
                    typeof( KhaldunZealot ),
                    typeof( KhaldunSummoner ),
                    typeof( LichLord ),
                    typeof( SkeletalKnight ),
                    typeof( SummonedDaemon ),
                    typeof( SummonedEarthElemental ),
                    typeof( SummonedWaterElemental ),
                    typeof( SummonedFireElemental ),
                    typeof( MeerWarrior ),
                    typeof( MeerEternal ),
                    typeof( MeerMage ),
                    typeof( MeerCaptain ),
                    typeof( JukaLord ),
                    typeof( JukaMage ),
                    typeof( JukaWarrior ),
                    typeof( Cursed ),
                    typeof( GrimmochDrummel ),
                    typeof( LysanderGathenwale ),
                    typeof( MorgBergen ),
                    typeof( ShadowFiend ),
                    typeof( SpectralArmour ),
                    typeof( TavaraSewel ),
                    typeof( ArcaneDaemon ),
                    typeof( Doppleganger ),
                    typeof( EnslavedGargoyle ),
                    typeof( ExodusMinion ),
                    typeof( ExodusOverseer ),
                    typeof( GargoyleDestroyer ),
                    typeof( GargoyleEnforcer ),
                    typeof( Moloch ),
                    typeof( Gaman ),
                    typeof( Lich ),
                    typeof( OphidianArchmage ),
                    typeof( OphidianMage ),
                    typeof( OphidianWarrior ),
                    typeof( OphidianMatriarch ),
                    typeof( OphidianKnight ),
                    typeof( PoisonElemental ),
                    typeof( ElderPoisonElemental ),
                    typeof( SandVortex ),
                    typeof( Leviathan ),
                    typeof( DreadSpider ),
                    typeof( LordOaks ),
                    typeof( Silvani ),
                    typeof( SilverSerpent ),
                    typeof( SandVortex ),
                    typeof( Revenant ),
                    typeof( DemonKnight ),
                    typeof( LadyOfTheSnow ),
                    typeof( RaiJu ),
                    typeof( Ronin ),
                    typeof( RuneBeetle ),
                    typeof( CapturedHordeMinion ),
                    typeof( ServantOfSemidar ),
                    typeof( Minotaur ),
                    typeof( MinotaurCaptain ),
                    typeof( MinotaurScout ),
                    typeof( FetidEssence ),
                    typeof( Satyr ),
                    typeof( MLDryad ),
                    typeof( CorruptedSoul ),
                    typeof( FeralTreefellow ),
                    typeof( PestilentBandage ),
                    typeof( TormentedMinotaur ),
                    typeof( Troglodyte ),
                    typeof( BloodTroll ),
                    typeof( GrayTroll ),
                    typeof( MercuryGazer ),
                    typeof( SilverDaemon ),
                    typeof( SilverDaemonLord ),
                    typeof( MongbatLord ),
                    typeof( OrcishExecutioner ),
                    typeof( ElderMojoka ),
                    typeof( OrcMojoka ),
                    typeof( OrcishSurjin ),
                    typeof( OrcishPeon ),
                    typeof( OrcishGrunt ),
                    typeof( OrcishScout ),
                    typeof( OrcishMaurk ),
                    typeof( DespiseOrc ),
                    typeof( UndeadOgreLord ),
                    typeof( HollowOne ),
                    typeof( AtlanteanWarden),
                    typeof( AtlanteanBattleMage),
                    typeof( Custom.Pirates.OceanFisherman),
                    typeof( Custom.Pirates.OceanPirate),
                    typeof( Custom.Pirates.OceanPirateCaptain),
                    typeof( Custom.Pirates.BritainSailor),
                    typeof( Custom.Pirates.BritainMarine),
                    typeof( Custom.Pirates.BritainShipCaptain),
                    typeof( Custom.Pirates.BritainShipCarpenter),
                    typeof( Custom.Pirates.BritainShipSurgeon),
                    typeof( Custom.Pirates.PirateShipCarpenter),
                    typeof( Custom.Pirates.PirateSawbones),
                    typeof( Custom.Pirates.SkeletalCaptain),
                    typeof( Custom.Pirates.SkeletalCrewman),
                    typeof( Custom.Pirates.GhostShipNecromancer),
                    typeof( Cockatrice),
                    typeof( CoralSnake),
                    typeof( GiantCoralSnake),
                    typeof( ForestSpider ),
                    typeof( GiantBat ),
                    typeof( VampireBat ),
                    typeof( GreaterLizard ),
                    typeof( Locust ),
                    typeof( ColossusTermite ),
                    typeof( RockSpider ),
                    typeof( Salamander ),
                    typeof( ScorpionHatchling ),
                    typeof( ShadowDragon ),
                    typeof( ChromaticDragon ),
                    typeof( WyvernHatchling ),
                    typeof( Umberhulk ),
                    typeof( KuoToa ),
                    typeof( DriderWarrior ),
                    typeof( DriderSentinel ),
                    typeof( DriderHarbinger ),
                    typeof( DrowBlademaster ),
                    typeof( DrowBlackguard ),
                    typeof( DrowSpellsinger ),
                    typeof( MindFlayer ),
                    typeof( OrghereimBeastmaster),
                    typeof( OrghereimCrone),
                    typeof( OrghereimBoneMender),
                    typeof( OrghereimIceCarl),
                    typeof( OrghereimSage),
                    typeof( OrghereimSwordThane),
                    typeof( OrghereimTracker),
                    typeof( OrghereimBowMaiden),
                    typeof( OrghereimShieldMaiden),
                    typeof( OrghereimShieldMother),
                    typeof( Maggot ),
                    typeof( Entrail ),
                    typeof( DiseasedViscera ),
                    typeof( Phoenix ),
                    typeof( Eagle ),
                    typeof( EarthlyTendril ),
                    typeof( PitTentacle ),
                    typeof( GiantRotworm ),
                    typeof( RotwormLarva ),

                    typeof( Custom.HenchmanPirate ),
                    typeof( Custom.HenchmanRaider ),
                    typeof( Custom.HenchmanPirateBoatswain ),
                    typeof( Custom.HenchmanPirateFirstMate ),
                    typeof( Custom.HenchmanSailor ),
                    typeof( Custom.HenchmanMarine ),
                    typeof( Custom.HenchmanNavyBoatswain ),
                    typeof( Custom.HenchmanNavyFirstMate ),
                    typeof( Custom.HenchmanSquire ),
                    typeof( Custom.HenchmanKnight ),
                    typeof( Custom.HenchmanPaladin ),
                    typeof( Custom.HenchmanCrusader ),
                    typeof( Custom.HenchmanBandit ),
                    typeof( Custom.HenchmanMercenary ),
                    typeof( Custom.HenchmanAssassin ),
                    typeof( Custom.HenchmanShadowblade ),
                    typeof( Custom.HenchmanPirateCarpenter ),
                    typeof( Custom.HenchmanNavyCarpenter ),
                    typeof( DeepTentacle ),
                    typeof( DeDOSNetGremlin ),
                    typeof( DeDOSBot ),
                    typeof( DeDOSLargeBot ),
                    typeof( DeDOSMassiveBot ),
                    typeof( DeDOSTunneler ),
                    typeof( Custom.HenchmanBoneMagi ),
                    typeof( Custom.HenchmanLich ),
                    typeof( Custom.HenchmanMummy ),
                    typeof( Custom.HenchmanRottingCorpse ),
                    typeof( Custom.HenchmanSkeletalKnight ),
                    typeof( Custom.HenchmanSkeleton ),
                    typeof( Custom.HenchmanSpectre ),
                    typeof( Custom.HenchmanVampireCountess ),
                    typeof( Custom.HenchmanVampireThrall ),
                    typeof( Custom.HenchmanZombie ),
                    typeof( Lodestone ),
                    typeof( RockGuar ),
                    typeof( BloodStalker ),                    
                } ),  
            
                //Fast
				new SpeedInfo( 0.35, 0.45, new Type[]
                {
                    typeof( SummonedAirElemental ),
                    typeof( AirElemental ),
                    typeof( Gust ),
                    typeof( BlackBear ),
                    typeof( Alligator ),
                    typeof( BrownBear ),
                    typeof( Gorilla ),
                    typeof( WhiteWyrm ),
                    typeof( AncientWinterWyrm ),
                    typeof( GiantBlackWidow ),
                    typeof( AncientWyrm ),
                    typeof( Balron ),
                    typeof( Dragon ),
                    typeof( SavageRider ),
                    typeof( AbysmalHorror ),
                    typeof( BoneDemon ),
                    typeof( Devourer ),
                    typeof( FleshGolem ),
                    typeof( Gibberling ),
                    typeof( GoreFiend ),
                    typeof( Impaler ),
                    typeof( PatchworkSkeleton ),
                    typeof( Ravager ),
                    typeof( ShadowKnight ),
                    typeof( SkitteringHopper ),
                    typeof( Treefellow ),
                    typeof( VampireBat ),
                    typeof( WailingBanshee ),
                    typeof( WandererOfTheVoid ),
                    typeof( BakeKitsune ),
                    typeof( DeathwatchBeetleHatchling ),
                    typeof( Kappa ),
                    typeof( KazeKemono ),
                    typeof( DeathwatchBeetle ),
                    typeof( TsukiWolf ),
                    typeof( YomotsuElder ),
                    typeof( YomotsuPriest ),
                    typeof( YomotsuWarrior ),
                    typeof( RevenantLion ),
                    typeof( Oni ),
                    typeof( DoubloonDockGuard ),
                    typeof( TurnableCannonGuard ),
                    typeof( Dockmaster ),
                    typeof( Healer ),
                    typeof( PricedHealer ),
                    typeof( FortuneTeller ),
                    typeof( EvilHealer ),
                    typeof( EvilWanderingHealer ),
                    typeof( WanderingHealer ),
                    typeof( TalkingBaseEscortable ),
                    typeof( SeekerOfAdventure ),
                    typeof( Peasant ),
                    typeof( Noble ),
                    typeof( Messenger ),
                    typeof( Merchant ),
                    typeof( EscortableMage ),
                    typeof( BrideGroom ),
                    typeof( GreaterDragon ),
                    typeof( Custom.Pirates.PirateCaptain ),
                    typeof( Custom.Pirates.Pirate ),
                    typeof( Savage ),
                    typeof( Brigand ),
                    typeof( SavageShaman ),
                    typeof( InterredGrizzle),
                    typeof( RagingGrizzlyBear ),
                    typeof( GoldenBalron ),
                    typeof( AncientRedWyrm ),
                    typeof( AcidElemental ),
                    typeof( ElderAcidElemental ),
                    typeof( BlackOrc ),
                    typeof( GreaterMongbat ),
                    typeof( UndeadKnight ),
                    typeof( SanguinAssassin ),
                    typeof( SanguinConscript ),
                    typeof( SanguinDefender ),
                    typeof( SanguinHealer ),
                    typeof( SanguinKnight ),
                    typeof( SanguinMage),
                    typeof( SanguinMedic ),
                    typeof( SanguinMender ),
                    typeof( SanguinProtector ),
                    typeof( SanguinWizard ),
                    typeof( SanguinHunter ),
                    typeof( SanguinAlchemist ),
                    typeof( ArcaneDragon ),
                    typeof( Bullvore ),
                    typeof( SvirfneblinIllusionist ),
                    typeof( SvirfneblinRogue ),
                    typeof( CorruptSpiderling ),
                    typeof( Deathspinner ),
                    typeof( AncientNecromancer ),
                    typeof( KhaldunLichAlmonjin ),
                    typeof( KhaldunLichKaltivel ),
                    typeof( KhaldunLichBaratoz ),
                    typeof( KhaldunLichMaliel ),
                    typeof( KhaldunLichAnshu ),
                    typeof( AncientHellhound ),
                    typeof( ArmoredCharger ),
                    typeof( Bloodworm ),
                    typeof( GiantDemon ),
                    typeof( GreaterPhoenix ),
                    typeof( HugeDragon ),
                    typeof( Hydra ),
                    typeof( PrimalLich ),
                    typeof( RisingColossus ),
                    typeof( Rotworm ),
                    typeof( StoneGolem ),
                    typeof( Turkey ),
                    typeof( Poacher ),
                    typeof( GraveRobber ),
                    typeof( TombRaider ),
                    typeof( Thug ),
                    typeof( Bootlegger ),
                    typeof( Smuggler ),
                    typeof( EasterBunny ),
                    typeof( WildOne ),
                    typeof( GreatGobbler ),
                    typeof( LesserGobbler ),
                    typeof( LavaElemental ),
                    typeof( DeDOSKynDragon ),
                } ),

                new SpeedInfo( 0.3, 0.5, new Type[] {
                    typeof( ArmoredTitan ),
                }),

				// Very Fast
				new SpeedInfo( 0.3, 0.4, new Type[]
                {
                    typeof( Cougar ),
                    typeof( Cat ),
                    typeof( DireWolf ),
                    typeof( Dog ),
                    typeof( Dolphin ),
                    typeof( DesertOstard ),
                    typeof( FireSteed ),
                    typeof( ForestOstard ),
                    typeof( FrenziedOstard ),
                    typeof( GreatHart ),
                    typeof( Caribou ),
                    typeof( GreyWolf ),
                    typeof( HellHound ),
                    typeof( IceSkitter ),
                    typeof( Deepstalker ),
                    typeof( Hind ),
                    typeof( Horse ),
                    typeof( PackHorse ),
                    typeof( PackLlama ),
                    typeof( Panther ),
                    typeof( Rabbit ),
                    typeof( RidableLlama ),
                    typeof( Ridgeback ),
                    typeof( SilverSteed ),
                    typeof( Bird ),
                    typeof( Crane ),
                    typeof( SavageRidgeback ),
                    typeof( WhiteWolf ),
                    typeof( TBWarHorse ),
                    typeof( CoMWarHorse ),
                    typeof( MinaxWarHorse ),
                    typeof( SLWarHorse ),
                    typeof( Unicorn ),
                    typeof( TimberWolf ),
                    typeof( SnowLeopard ),
                    typeof( HellCat ),
                    typeof( SkeletalMount ),
                    typeof( EtherealWarrior ),
                    typeof( Nightmare ),
                    typeof( Wisp ),
                    typeof( LesserHiryu ),
                    typeof( GrizzlyBear ),
                    typeof( PolarBear ),
                    typeof( Hiryu ),
                    typeof( WanderingHealer ),
                    typeof( FleshRenderer ),
                    typeof( FireBeetle ),
                    typeof( FanDancer ),
                    typeof( EliteNinja ),
                    typeof( UnholyFamiliar ),
                    typeof( UnholySteed ),
                    typeof( HolySteed ),
                    typeof( HolyFamiliar ),
                    typeof( Parrot ), 
                    typeof( ArcherGuard ),
                    typeof( WarriorGuard ),
                    typeof( CuSidhe ),
                    typeof( HarrowerTentacles ),
                    typeof( EnergyLlama ),
                    typeof( DarkWisp ),
                    typeof( DragonWhelp ),
                    typeof( DrakeWhelp ),
                    typeof( BayingHound ),
                    typeof( Ferret ),
                    typeof( Fox ),
                    typeof( Skitter ),
                    typeof( HookHorror ),
                    typeof( Efreet ),
                    typeof( DemonwebQueen ),
                    typeof( Atraxis ),
                    typeof( DrowKeeper ),
                    typeof( ElderWardr ),
                    typeof( EternalFlameWyrm ),
                    typeof( HydraKing ),
                    typeof( OrcWarboss ),
                    typeof( ShadowOfPeradun ),
                    typeof( Bloodcat ),
                    typeof( DarkSentinel ),
                    typeof( DaemonicOverlord ),
                    typeof( WoodlandSprite ),
                    typeof( ElderWoodlandSprite ),
                    typeof( WoodlandDruid ),
                    typeof( MysteryDaemon ),
                    typeof( Sabertusk ),
                    typeof( Peradun ),
                    typeof( DeDOSTrojanHorse ),
                } ),

                new SpeedInfo( 0.35, 0.35, new Type[]
                {
                    typeof( SanguinArchblade ),
                } ),

                //Super Fast
                new SpeedInfo( 0.25, 0.35, new Type[]
                {
                    typeof( Valkyrie),
                    typeof( MysteryVampire ),
                    typeof( MysteryMedusa ),
                    typeof( EmperorDragon ),
                    typeof( TheDeepOne ),

                    typeof( Sanguineous ),
                    typeof(BloodCourser),
                } ),

				//Ultra
				new SpeedInfo( 0.175, 0.3, new Type[]
                {
                    typeof( Barracoon ),
                    typeof( Mephitis ),
                    typeof( Neira ),
                    typeof( Rikktor ),
                    typeof( Semidar ),
                    typeof( Pixie ),
                    typeof( VorpalBunny ),
                    typeof( KhaldunRevenant ),
                    typeof( Ilhenir ),
                    typeof( Meraktus ),
                    typeof( Twaulo ),                   
                } ),

                //Scouts
				new SpeedInfo( 0.3, 0.3, new Type[]
                {
                    typeof( SanguinScout ),
                } )
            };
    }
}