using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
#if Framework_4_0
using System.Linq;
using System.Threading.Tasks;
#endif
using Server;
using Server.Network;
using Server.Items;
using Server.ContextMenus;

namespace Server
{
    #region Layer
    /// <summary>
    /// Enumeration of item layer values.
    /// </summary>
    public enum Layer : byte
    {
        /// <summary>
        /// Invalid layer.
        /// </summary>
        Invalid = 0x00,
        /// <summary>
        /// First valid layer. Equivalent to <c>Layer.OneHanded</c>.
        /// </summary>
        FirstValid = 0x01,
        /// <summary>
        /// One handed weapon.
        /// </summary>
        OneHanded = 0x01,
        /// <summary>
        /// Two handed weapon or shield.
        /// </summary>
        TwoHanded = 0x02,
        /// <summary>
        /// Shoes.
        /// </summary>
        Shoes = 0x03,
        /// <summary>
        /// Pants.
        /// </summary>
        Pants = 0x04,
        /// <summary>
        /// Shirts.
        /// </summary>
        Shirt = 0x05,
        /// <summary>
        /// Helmets, hats, and masks.
        /// </summary>
        Helm = 0x06,
        /// <summary>
        /// Gloves.
        /// </summary>
        Gloves = 0x07,
        /// <summary>
        /// Rings.
        /// </summary>
        Ring = 0x08,
        /// <summary>
        /// Talismans.
        /// </summary>
        Talisman = 0x09,
        /// <summary>
        /// Gorgets and necklaces.
        /// </summary>
        Neck = 0x0A,
        /// <summary>
        /// Hair.
        /// </summary>
        Hair = 0x0B,
        /// <summary>
        /// Half aprons.
        /// </summary>
        Waist = 0x0C,
        /// <summary>
        /// Torso, inner layer.
        /// </summary>
        InnerTorso = 0x0D,
        /// <summary>
        /// Bracelets.
        /// </summary>
        Bracelet = 0x0E,
        /// <summary>
        /// Unused.
        /// </summary>
        Unused_xF = 0x0F,
        /// <summary>
        /// Beards and mustaches.
        /// </summary>
        FacialHair = 0x10,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        MiddleTorso = 0x11,
        /// <summary>
        /// Earings.
        /// </summary>
        Earrings = 0x12,
        /// <summary>
        /// Arms and sleeves.
        /// </summary>
        Arms = 0x13,
        /// <summary>
        /// Cloaks.
        /// </summary>
        Cloak = 0x14,
        /// <summary>
        /// Backpacks.
        /// </summary>
        Backpack = 0x15,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        OuterTorso = 0x16,
        /// <summary>
        /// Leggings, outer layer.
        /// </summary>
        OuterLegs = 0x17,
        /// <summary>
        /// Leggings, inner layer.
        /// </summary>
        InnerLegs = 0x18,
        /// <summary>
        /// Last valid non-internal layer. Equivalent to <c>Layer.InnerLegs</c>.
        /// </summary>
        LastUserValid = 0x18,
        /// <summary>
        /// Mount item layer.
        /// </summary>
        Mount = 0x19,
        /// <summary>
        /// Vendor 'buy pack' layer.
        /// </summary>
        ShopBuy = 0x1A,
        /// <summary>
        /// Vendor 'resale pack' layer.
        /// </summary>
        ShopResale = 0x1B,
        /// <summary>
        /// Vendor 'sell pack' layer.
        /// </summary>
        ShopSell = 0x1C,
        /// <summary>
        /// Bank box layer.
        /// </summary>
        Bank = 0x1D,
        /// <summary>
        /// Last valid layer. Equivalent to <c>Layer.Bank</c>.
        /// </summary>
        LastValid = 0x1D
    }

    #endregion

    /// <summary>
    /// Internal flags used to signal how the item should be updated and resent to nearby clients.
    /// </summary>
    [Flags]
    public enum ItemDelta
    {
        /// <summary>
        /// Nothing.
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// Resend the item.
        /// </summary>
        Update = 0x00000001,
        /// <summary>
        /// Resend the item only if it is equiped.
        /// </summary>
        EquipOnly = 0x00000002,
        /// <summary>
        /// Resend the item's properties.
        /// </summary>
        Properties = 0x00000004
    }

    public enum Quality
    {
        Low,
        Regular,
        Exceptional
    }

    public enum CraftResource
    {
        None = 0,
        Iron = 1,
        DullCopper,
        ShadowIron,
        Copper,
        Bronze,
        Gold,
        Agapite,
        Verite,
        Valorite,
        Lunite,

        RegularLeather = 101,
        SpinedLeather,
        HornedLeather,
        BarbedLeather,

        RegularWood = 301,
        OakWood,
        AshWood,
        YewWood,
        Heartwood,
        Bloodwood,
        Frostwood
    }

    public enum CraftResourceType
    {
        None,
        Metal,
        Leather,
        Wood
    }

    public enum DungeonEnum
    {
        None,

        Shame,
        Deceit,
        Destard,
        Hythloth,
        Covetous,
        Wrong,
        Despise,
        Ice,
        Fire
    }

    public enum ItemGroupType
    {
        None,
        Regular,
        Crafted,
        Donation,
        Achievement,
        Loot,
        EventLoot,
        CurrencyReward
    }

    public enum ItemRarityType
    {
        None,
        Common,
        Uncommon,
        Rare,
        VeryRare,
        UltraRare
    }

    /// <summary>
    /// Enumeration containing possible ways to handle item ownership on death.
    /// </summary>
    public enum DeathMoveResult
    {
        /// <summary>
        /// The item should be placed onto the corpse.
        /// </summary>
        MoveToCorpse,
        /// <summary>
        /// The item should remain equiped.
        /// </summary>
        RemainEquiped,
        /// <summary>
        /// The item should be placed into the owners backpack.
        /// </summary>
        MoveToBackpack,

        /// <summary>
        /// The item should will be deleted.
        /// </summary>
        Delete
    }

    /// <summary>
    /// Enumeration containing all possible light types. These are only applicable to light source items, like lanterns, candles, braziers, etc.
    /// </summary>
    public enum LightType
    {
        /// <summary>
        /// Window shape, arched, ray shining east.
        /// </summary>
        ArchedWindowEast,
        /// <summary>
        /// Medium circular shape.
        /// </summary>
        Circle225,
        /// <summary>
        /// Small circular shape.
        /// </summary>
        Circle150,
        /// <summary>
        /// Door shape, shining south.
        /// </summary>
        DoorSouth,
        /// <summary>
        /// Door shape, shining east.
        /// </summary>
        DoorEast,
        /// <summary>
        /// Large semicircular shape (180 degrees), north wall.
        /// </summary>
        NorthBig,
        /// <summary>
        /// Large pie shape (90 degrees), north-east corner.
        /// </summary>
        NorthEastBig,
        /// <summary>
        /// Large semicircular shape (180 degrees), east wall.
        /// </summary>
        EastBig,
        /// <summary>
        /// Large semicircular shape (180 degrees), west wall.
        /// </summary>
        WestBig,
        /// <summary>
        /// Large pie shape (90 degrees), south-west corner.
        /// </summary>
        SouthWestBig,
        /// <summary>
        /// Large semicircular shape (180 degrees), south wall.
        /// </summary>
        SouthBig,
        /// <summary>
        /// Medium semicircular shape (180 degrees), north wall.
        /// </summary>
        NorthSmall,
        /// <summary>
        /// Medium pie shape (90 degrees), north-east corner.
        /// </summary>
        NorthEastSmall,
        /// <summary>
        /// Medium semicircular shape (180 degrees), east wall.
        /// </summary>
        EastSmall,
        /// <summary>
        /// Medium semicircular shape (180 degrees), west wall.
        /// </summary>
        WestSmall,
        /// <summary>
        /// Medium semicircular shape (180 degrees), south wall.
        /// </summary>
        SouthSmall,
        /// <summary>
        /// Shaped like a wall decoration, north wall.
        /// </summary>
        DecorationNorth,
        /// <summary>
        /// Shaped like a wall decoration, north-east corner.
        /// </summary>
        DecorationNorthEast,
        /// <summary>
        /// Small semicircular shape (180 degrees), east wall.
        /// </summary>
        EastTiny,
        /// <summary>
        /// Shaped like a wall decoration, west wall.
        /// </summary>
        DecorationWest,
        /// <summary>
        /// Shaped like a wall decoration, south-west corner.
        /// </summary>
        DecorationSouthWest,
        /// <summary>
        /// Small semicircular shape (180 degrees), south wall.
        /// </summary>
        SouthTiny,
        /// <summary>
        /// Window shape, rectangular, no ray, shining south.
        /// </summary>
        RectWindowSouthNoRay,
        /// <summary>
        /// Window shape, rectangular, no ray, shining east.
        /// </summary>
        RectWindowEastNoRay,
        /// <summary>
        /// Window shape, rectangular, ray shining south.
        /// </summary>
        RectWindowSouth,
        /// <summary>
        /// Window shape, rectangular, ray shining east.
        /// </summary>
        RectWindowEast,
        /// <summary>
        /// Window shape, arched, no ray, shining south.
        /// </summary>
        ArchedWindowSouthNoRay,
        /// <summary>
        /// Window shape, arched, no ray, shining east.
        /// </summary>
        ArchedWindowEastNoRay,
        /// <summary>
        /// Window shape, arched, ray shining south.
        /// </summary>
        ArchedWindowSouth,
        /// <summary>
        /// Large circular shape.
        /// </summary>
        Circle300,
        /// <summary>
        /// Large pie shape (90 degrees), north-west corner.
        /// </summary>
        NorthWestBig,
        /// <summary>
        /// Negative light. Medium pie shape (90 degrees), south-east corner.
        /// </summary>
        DarkSouthEast,
        /// <summary>
        /// Negative light. Medium semicircular shape (180 degrees), south wall.
        /// </summary>
        DarkSouth,
        /// <summary>
        /// Negative light. Medium pie shape (90 degrees), north-west corner.
        /// </summary>
        DarkNorthWest,
        /// <summary>
        /// Negative light. Medium pie shape (90 degrees), south-east corner. Equivalent to <c>LightType.SouthEast</c>.
        /// </summary>
        DarkSouthEast2,
        /// <summary>
        /// Negative light. Medium circular shape (180 degrees), east wall.
        /// </summary>
        DarkEast,
        /// <summary>
        /// Negative light. Large circular shape.
        /// </summary>
        DarkCircle300,
        /// <summary>
        /// Opened door shape, shining south.
        /// </summary>
        DoorOpenSouth,
        /// <summary>
        /// Opened door shape, shining east.
        /// </summary>
        DoorOpenEast,
        /// <summary>
        /// Window shape, square, ray shining east.
        /// </summary>
        SquareWindowEast,
        /// <summary>
        /// Window shape, square, no ray, shining east.
        /// </summary>
        SquareWindowEastNoRay,
        /// <summary>
        /// Window shape, square, ray shining south.
        /// </summary>
        SquareWindowSouth,
        /// <summary>
        /// Window shape, square, no ray, shining south.
        /// </summary>
        SquareWindowSouthNoRay,
        /// <summary>
        /// Empty.
        /// </summary>
        Empty,
        /// <summary>
        /// Window shape, skinny, no ray, shining south.
        /// </summary>
        SkinnyWindowSouthNoRay,
        /// <summary>
        /// Window shape, skinny, ray shining east.
        /// </summary>
        SkinnyWindowEast,
        /// <summary>
        /// Window shape, skinny, no ray, shining east.
        /// </summary>
        SkinnyWindowEastNoRay,
        /// <summary>
        /// Shaped like a hole, shining south.
        /// </summary>
        HoleSouth,
        /// <summary>
        /// Shaped like a hole, shining south.
        /// </summary>
        HoleEast,
        /// <summary>
        /// Large circular shape with a moongate graphic embeded.
        /// </summary>
        Moongate,
        /// <summary>
        /// Unknown usage. Many rows of slightly angled lines.
        /// </summary>
        Strips,
        /// <summary>
        /// Shaped like a small hole, shining south.
        /// </summary>
        SmallHoleSouth,
        /// <summary>
        /// Shaped like a small hole, shining east.
        /// </summary>
        SmallHoleEast,
        /// <summary>
        /// Large semicircular shape (180 degrees), north wall. Identical graphic as <c>LightType.NorthBig</c>, but slightly different positioning.
        /// </summary>
        NorthBig2,
        /// <summary>
        /// Large semicircular shape (180 degrees), west wall. Identical graphic as <c>LightType.WestBig</c>, but slightly different positioning.
        /// </summary>
        WestBig2,
        /// <summary>
        /// Large pie shape (90 degrees), north-west corner. Equivalent to <c>LightType.NorthWestBig</c>.
        /// </summary>
        NorthWestBig2
    }

    /// <summary>
    /// Enumeration of an item's loot and steal state.
    /// </summary>
    public enum LootType : byte
    {
        /// <summary>
        /// Stealable. Lootable.
        /// </summary>
        Regular = 0,
        /// <summary>
        /// Unstealable. Unlootable, unless owned by a murderer.
        /// </summary>
        Newbied = 1,
        /// <summary>
        /// Unstealable. Unlootable, always.
        /// </summary>
        Blessed = 2,
        /// <summary>
        /// Stealable. Lootable, always.
        /// </summary>
        Cursed = 3
    }

    public class BounceInfo
    {
        public Map m_Map;
        public Point3D m_Location, m_WorldLoc;
        public object m_Parent;

        public BounceInfo(Item item)
        {
            m_Map = item.Map;
            m_Location = item.Location;
            m_WorldLoc = item.GetWorldLocation();
            m_Parent = item.Parent;
        }

        private BounceInfo(Map map, Point3D loc, Point3D worldLoc, object parent)
        {
            m_Map = map;
            m_Location = loc;
            m_WorldLoc = worldLoc;
            m_Parent = parent;
        }

        public static BounceInfo Deserialize(GenericReader reader)
        {
            if (reader.ReadBool())
            {
                Map map = reader.ReadMap();
                Point3D loc = reader.ReadPoint3D();
                Point3D worldLoc = reader.ReadPoint3D();

                object parent;

                Serial serial = reader.ReadInt();

                if (serial.IsItem)
                    parent = World.FindItem(serial);
                else if (serial.IsMobile)
                    parent = World.FindMobile(serial);
                else
                    parent = null;

                return new BounceInfo(map, loc, worldLoc, parent);
            }
            else
            {
                return null;
            }
        }

        public static void Serialize(BounceInfo info, GenericWriter writer)
        {
            if (info == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);

                writer.Write(info.m_Map);
                writer.Write(info.m_Location);
                writer.Write(info.m_WorldLoc);

                if (info.m_Parent is Mobile)
                    writer.Write((Mobile)info.m_Parent);
                else if (info.m_Parent is Item)
                    writer.Write((Item)info.m_Parent);
                else
                    writer.Write((Serial)0);
            }
        }
    }

    public enum TotalType
    {
        Gold,
        Items,
        Weight,
        GhostCoins,
    }

    public enum PlayerClass
    {
        None,
        Paladin,
        Murderer,
        Pirate,
        Thief,
        Detective,
        Crafter,
        Tamer,
        MonsterHunter,
        TreasureHunter
    }

    [Flags]
    public enum ExpandFlag
    {
        None = 0x000,

        Name = 0x001,
        Items = 0x002,
        Bounce = 0x004,
        Holder = 0x008,
        Blessed = 0x010,
        TempFlag = 0x020,
        SaveFlag = 0x040,
        Weight = 0x080,
        Spawner = 0x100
    }

    public class Item : IEntity, IHued, IComparable<Item>, ISerializable, ISpawnable
    {
        // IPY ARENA HUE
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public int OriginalHue { set; get; }
        // IPY ARENA HUE

        //UOAC Item Reforging system
        [CommandProperty(AccessLevel.GameMaster)]
        public int BreakChanceFromReforge { get; set; }

        private bool m_DonationItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DonationItem { get { return m_DonationItem; } set { m_DonationItem = value; } }
        public static readonly List<Item> EmptyItems = new List<Item>();

        public int CompareTo(IEntity other)
        {
            if (other == null)
                return -1;

            return m_Serial.CompareTo(other.Serial);
        }

        public int CompareTo(Item other)
        {
            return this.CompareTo((IEntity)other);
        }

        public int CompareTo(object other)
        {
            if (other == null || other is IEntity)
                return this.CompareTo((IEntity)other);

            throw new ArgumentException();
        }

        #region Standard fields
        private Serial m_Serial;
        private Point3D m_Location;
        private int m_ItemID;
        private int m_Hue;
        private int m_Amount;
        private Layer m_Layer;
        private object m_Parent; // Mobile, Item, or null=World
        private Map m_Map;
        private LootType m_LootType;
        private DateTime m_LastMovedTime;
        private Direction m_Direction;
        #endregion

        private ItemDelta m_DeltaFlags;
        private ImplFlag m_Flags;

        public AcquisitionType Acquisition = AcquisitionType.Other;
        public int AcquisitionData = 0;

        #region Packet caches
        private Packet m_WorldPacket;
        private Packet m_WorldPacketSA;
        private Packet m_WorldPacketHS;
        private Packet m_RemovePacket;

        private Packet m_OPLPacket;
        private ObjectPropertyList m_PropertyList;
        #endregion

        public int TempFlags
        {
            get
            {
                CompactInfo info = LookupCompactInfo();

                if (info != null)
                    return info.m_TempFlags;

                return 0;
            }
            set
            {
                CompactInfo info = AcquireCompactInfo();

                info.m_TempFlags = value;

                if (info.m_TempFlags == 0)
                    VerifyCompactInfo();
            }
        }

        public int SavedFlags
        {
            get
            {
                CompactInfo info = LookupCompactInfo();

                if (info != null)
                    return info.m_SavedFlags;

                return 0;
            }
            set
            {
                CompactInfo info = AcquireCompactInfo();

                info.m_SavedFlags = value;

                if (info.m_SavedFlags == 0)
                    VerifyCompactInfo();
            }
        }

        public enum AcquisitionType : byte
        {
            Other = 0,
            Crafted = 1,
            VendorBought = 2
        }

        public virtual ItemGroupType BaseItemGroup { get { return ItemGroupType.Regular; } }

        private ItemGroupType m_ItemGroup = ItemGroupType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public ItemGroupType ItemGroup
        {
            get
            {
                if (m_ItemGroup == ItemGroupType.None)
                    return BaseItemGroup;

                return m_ItemGroup; 
            }

            set { m_ItemGroup = value; }
        }

        public virtual ItemRarityType BaseItemRarity { get { return ItemRarityType.Common; } }

        private ItemRarityType m_ItemRarity = ItemRarityType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public ItemRarityType ItemRarity
        {
            get 
            {
                if (m_ItemRarity == ItemRarityType.None)
                    return BaseItemRarity; 

                return m_ItemRarity; 
            }

            set { m_ItemRarity = value; }
        }

        private bool m_DecorativeEquipment = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DecorativeEquipment
        {
            get { return m_DecorativeEquipment; }
            set { m_DecorativeEquipment = value; }
        }

        private PlayerClass m_PlayerClass = PlayerClass.None;

        public virtual bool TelekinesisImmune
        {
            get { return false; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerClass PlayerClass
        {
            get { return m_PlayerClass; }
            set { m_PlayerClass = value; }
        }

        private bool m_PlayerClassRestricted = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerClassRestricted
        {
            get { return m_PlayerClassRestricted; }
            set { m_PlayerClassRestricted = value; }
        }

        private Mobile m_PlayerClassOwner;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile PlayerClassOwner
        {
            get { return m_PlayerClassOwner; }
            set { m_PlayerClassOwner = value; }
        }

        public virtual int PlayerClassCurrencyValue { get { return 0; } }
        public virtual bool AlwaysAllowDoubleClick { get { return false; } }

        private bool m_Stealable = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Stealable
        {
            get { return m_Stealable; }
            set
            {
                m_Stealable = value;
                if (value)
                    Movable = false;
            }
        }

        private bool m_AlreadyStolen = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AlreadyStolen
        {
            get { return m_AlreadyStolen; }
            set
            {
                m_AlreadyStolen = value;
                if (value)
                    Movable = true;
            }
        }

        private double m_MinimumStealing = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double MinimumStealing
        {
            get { return m_MinimumStealing; }
            set { m_MinimumStealing = value; }
        }

        private double m_MaximumStealing = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double MaximumStealing
        {
            get { return m_MaximumStealing; }
            set { m_MaximumStealing = value; }
        }

        #region Arcane Charges

        private bool m_Identified;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Identified
        {
            get { return m_Identified; }
            set { m_Identified = value; InvalidateProperties(); }
        }

        private bool m_ArcaneRechargable = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ArcaneRechargable
        {
            get { return m_ArcaneRechargable; }
            set { m_ArcaneRechargable = value; }
        }

        private int m_ArcaneCharges = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ArcaneCharges
        {
            get { return m_ArcaneCharges; }
            set { m_ArcaneCharges = value; }
        }

        private int m_ArcaneChargesMax = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ArcaneChargesMax
        {
            get { return m_ArcaneChargesMax; }
            set { m_ArcaneChargesMax = value; }
        }

        private int m_TierLevel = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TierLevel
        {
            get { return m_TierLevel; }
            set { m_TierLevel = value; }
        }

        private int m_Experience = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }

        #endregion

        private bool m_OceanStatic = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool OceanStatic
        {
            get { return m_OceanStatic; }
            set { m_OceanStatic = value; }
        }

        private bool m_ShipItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShipItem
        {
            get { return m_ShipItem; }
            set { m_ShipItem = value; }
        }

        private Mobile m_CraftedBy;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile CraftedBy
        {
            get { return m_CraftedBy; }
            set { m_CraftedBy = value; }
        }

        private string m_CrafterName = "";
        [CommandProperty(AccessLevel.GameMaster)]
        public string CrafterName
        {
            get { return m_CrafterName; }
            set { m_CrafterName = value; }
        }

        private bool m_DisplayCrafter = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DisplayCrafter
        {
            get { return m_DisplayCrafter; }
            set { m_DisplayCrafter = value; }
        }

        private Quality m_Quality = Quality.Regular;
        [CommandProperty(AccessLevel.GameMaster)]
        public Quality Quality
        {
            get { return m_Quality; }
            set
            {
                m_Quality = value;

                QualityChange();
            }
        }

        public virtual CraftResource DefaultResource { get { return CraftResource.None; } }

        private CraftResource m_Resource = CraftResource.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource;  }

            set
            {
                m_Resource = value;

                ResourceChange();
            }
        }

        private DungeonEnum m_Dungeon = DungeonEnum.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public DungeonEnum Dungeon
        {
            get { return m_Dungeon; }
            set
            {
                m_Dungeon = value;

                DungeonChange();
            }
        }

        public virtual void QualityChange()
        {
        }

        public virtual void ResourceChange()
        {
        }

        public virtual void DungeonChange()
        {
        }

        public virtual int GetArcaneEssenceValue()
        {
            return 0;
        }

        public virtual double GetSellValueScalar()
        {
            return 1.0;
        }

        public static string GetDungeonName(DungeonEnum dungeon)
        {
            switch (dungeon)
            {
                case DungeonEnum.Shame: return "Shame";
                case DungeonEnum.Deceit: return "Deceit";
                case DungeonEnum.Destard: return "Destard";
                case DungeonEnum.Hythloth: return "Hythloth";
                case DungeonEnum.Covetous: return "Covetous";
                case DungeonEnum.Wrong: return "Wrong";
                case DungeonEnum.Despise: return "Despise";
                case DungeonEnum.Ice: return "Ice";
                case DungeonEnum.Fire: return "Fire";
            }

            return "";
        }

        /// <summary>
        /// The <see cref="Mobile" /> who is currently <see cref="Mobile.Holding">holding</see> this item.
        /// </summary>
        public Mobile HeldBy
        {
            get
            {
                CompactInfo info = LookupCompactInfo();

                if (info != null)
                    return info.m_HeldBy;

                return null;
            }
            set
            {
                CompactInfo info = AcquireCompactInfo();

                info.m_HeldBy = value;

                if (info.m_HeldBy == null)
                    VerifyCompactInfo();
            }
        }

        [Flags]
        private enum ImplFlag : byte
        {
            None = 0x00,
            Visible = 0x01,
            Movable = 0x02,
            Deleted = 0x04,
            Stackable = 0x08,
            InQueue = 0x10,
            Insured = 0x20,
            PayedInsurance = 0x40,
            QuestItem = 0x80
        }

        private class CompactInfo
        {
            public string m_Name;

            public List<Item> m_Items;
            public BounceInfo m_Bounce;

            public Mobile m_HeldBy;
            public Mobile m_BlessedFor;

            public ISpawner m_Spawner;

            public int m_TempFlags;
            public int m_SavedFlags;

            public double m_Weight = -1;
        }

        private CompactInfo m_CompactInfo;

        public ExpandFlag GetExpandFlags()
        {
            CompactInfo info = LookupCompactInfo();

            ExpandFlag flags = 0;

            if (info != null)
            {
                if (info.m_BlessedFor != null)
                    flags |= ExpandFlag.Blessed;

                if (info.m_Bounce != null)
                    flags |= ExpandFlag.Bounce;

                if (info.m_HeldBy != null)
                    flags |= ExpandFlag.Holder;

                if (info.m_Items != null)
                    flags |= ExpandFlag.Items;

                if (info.m_Name != null)
                    flags |= ExpandFlag.Name;

                if (info.m_Spawner != null)
                    flags |= ExpandFlag.Spawner;

                if (info.m_SavedFlags != 0)
                    flags |= ExpandFlag.SaveFlag;

                if (info.m_TempFlags != 0)
                    flags |= ExpandFlag.TempFlag;

                if (info.m_Weight != -1)
                    flags |= ExpandFlag.Weight;
            }

            return flags;
        }

        private CompactInfo LookupCompactInfo()
        {
            return m_CompactInfo;
        }

        private CompactInfo AcquireCompactInfo()
        {
            if (m_CompactInfo == null)
                m_CompactInfo = new CompactInfo();

            return m_CompactInfo;
        }

        private void ReleaseCompactInfo()
        {
            m_CompactInfo = null;
        }

        private void VerifyCompactInfo()
        {
            CompactInfo info = m_CompactInfo;

            if (info == null)
                return;

            bool isValid = (info.m_Name != null)
                            || (info.m_Items != null)
                            || (info.m_Bounce != null)
                            || (info.m_HeldBy != null)
                            || (info.m_BlessedFor != null)
                            || (info.m_Spawner != null)
                            || (info.m_TempFlags != 0)
                            || (info.m_SavedFlags != 0)
                            || (info.m_Weight != -1);

            if (!isValid)
                ReleaseCompactInfo();
        }

        public List<Item> LookupItems()
        {
            if (this is Container)
                return (this as Container).m_Items;

            CompactInfo info = LookupCompactInfo();

            if (info != null)
                return info.m_Items;

            return null;
        }

        public List<Item> AcquireItems()
        {
            if (this is Container)
            {
                Container cont = this as Container;

                if (cont.m_Items == null)
                    cont.m_Items = new List<Item>();

                return cont.m_Items;
            }

            CompactInfo info = AcquireCompactInfo();

            if (info.m_Items == null)
                info.m_Items = new List<Item>();

            return info.m_Items;
        }

        private void SetFlag(ImplFlag flag, bool value)
        {
            if (value)
                m_Flags |= flag;
            else
                m_Flags &= ~flag;
        }

        private bool GetFlag(ImplFlag flag)
        {
            return ((m_Flags & flag) != 0);
        }

        public BounceInfo GetBounce()
        {
            CompactInfo info = LookupCompactInfo();

            if (info != null)
                return info.m_Bounce;

            return null;
        }

        public void RecordBounce()
        {
            CompactInfo info = AcquireCompactInfo();

            info.m_Bounce = new BounceInfo(this);
        }

        public void ClearBounce()
        {
            CompactInfo info = LookupCompactInfo();

            if (info != null)
            {
                BounceInfo bounce = info.m_Bounce;

                if (bounce != null)
                {
                    info.m_Bounce = null;

                    if (bounce.m_Parent is Item)
                    {
                        Item parent = (Item)bounce.m_Parent;

                        if (!parent.Deleted)
                            parent.OnItemBounceCleared(this);
                    }
                    else if (bounce.m_Parent is Mobile)
                    {
                        Mobile parent = (Mobile)bounce.m_Parent;

                        if (!parent.Deleted)
                            parent.OnItemBounceCleared(this);
                    }

                    VerifyCompactInfo();
                }
            }
        }

        /// <summary>
        /// Overridable. Virtual event invoked when a client, <paramref name="from" />, invokes a 'help request' for the Item. Seemingly no longer functional in newer clients.
        /// </summary>
        public virtual void OnHelpRequest(Mobile from)
        {
        }

        /// <summary>
        /// Overridable. Method checked to see if the item can be traded.
        /// </summary>
        /// <returns>True if the trade is allowed, false if not.</returns>
        public virtual bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return true;
        }

        /// <summary>
        /// Overridable. Virtual event invoked when a trade has completed, either successfully or not.
        /// </summary>
        public virtual void OnSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
        }

        /// <summary>
        /// Overridable. Method checked to see if the elemental resistances of this Item conflict with another Item on the <see cref="Mobile" />.
        /// </summary>
        /// <returns>
        /// <list type="table">
        /// <item>
        /// <term>True</term>
        /// <description>There is a confliction. The elemental resistance bonuses of this Item should not be applied to the <see cref="Mobile" /></description>
        /// </item>
        /// <item>
        /// <term>False</term>
        /// <description>There is no confliction. The bonuses should be applied.</description>
        /// </item>
        /// </list>
        /// </returns>
        public virtual bool CheckPropertyConfliction(Mobile m)
        {
            return false;
        }

        /// <summary>
        /// Overridable. Sends the <see cref="PropertyList">object property list</see> to <paramref name="from" />.
        /// </summary>
        public virtual void SendPropertiesTo(Mobile from)
        {
            from.Send(PropertyList);
        }

        /// <summary>
        /// Overridable. Adds the name of this item to the given <see cref="ObjectPropertyList" />. This method should be overriden if the item requires a complex naming format.
        /// </summary>
        public virtual void AddNameProperty(ObjectPropertyList list)
        {
            string name = this.Name;

            if (name == null)
            {
                if (m_Amount <= 1)
                    list.Add(LabelNumber);
                else
                    list.Add(1050039, "{0}\t#{1}", m_Amount, LabelNumber); // ~1_NUMBER~ ~2_ITEMNAME~
            }
            else
            {
                if (m_Amount <= 1)
                    list.Add(name);
                else
                    list.Add(1050039, "{0}\t{1}", m_Amount, Name); // ~1_NUMBER~ ~2_ITEMNAME~
            }
        }

        /// <summary>
        /// Overridable. Adds the loot type of this item to the given <see cref="ObjectPropertyList" />. By default, this will be either 'blessed', 'cursed', or 'insured'.
        /// </summary>
        public virtual void AddLootTypeProperty(ObjectPropertyList list)
        {
            if (m_LootType == LootType.Blessed)
                list.Add(1038021); // blessed
            else if (m_LootType == LootType.Cursed)
                list.Add(1049643); // cursed
            else if (Insured)
                list.Add(1061682); // <b>insured</b>
        }

        /// <summary>
        /// Overridable. Adds any elemental resistances of this item to the given <see cref="ObjectPropertyList" />.
        /// </summary>
        public virtual void AddResistanceProperties(ObjectPropertyList list)
        {
            int v = PhysicalResistance;

            if (v != 0)
                list.Add(1060448, v.ToString()); // physical resist ~1_val~%

            v = FireResistance;

            if (v != 0)
                list.Add(1060447, v.ToString()); // fire resist ~1_val~%

            v = ColdResistance;

            if (v != 0)
                list.Add(1060445, v.ToString()); // cold resist ~1_val~%

            v = PoisonResistance;

            if (v != 0)
                list.Add(1060449, v.ToString()); // poison resist ~1_val~%

            v = EnergyResistance;

            if (v != 0)
                list.Add(1060446, v.ToString()); // energy resist ~1_val~%
        }

        /// <summary>
        /// Overridable. Determines whether the item will show <see cref="AddWeightProperty" />. 
        /// </summary>
        public virtual bool DisplayWeight
        {
            get
            {
                if (!Core.ML)
                    return false;

                if (!Movable && !(IsLockedDown || IsSecure) && ItemData.Weight == 255)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Overridable. Displays cliloc 1072788-1072789. 
        /// </summary>
        public virtual void AddWeightProperty(ObjectPropertyList list)
        {
            int weight = this.PileWeight + this.TotalWeight;

            if (weight == 1)
            {
                list.Add(1072788, weight.ToString()); //Weight: ~1_WEIGHT~ stone
            }
            else
            {
                list.Add(1072789, weight.ToString()); //Weight: ~1_WEIGHT~ stones
            }
        }

        /// <summary>
        /// Overridable. Adds header properties. By default, this invokes <see cref="AddNameProperty" />, <see cref="AddBlessedForProperty" /> (if applicable), and <see cref="AddLootTypeProperty" /> (if <see cref="DisplayLootType" />).
        /// </summary>
        public virtual void AddNameProperties(ObjectPropertyList list)
        {
            AddNameProperty(list);

            if (IsSecure)
                AddSecureProperty(list);
            else if (IsLockedDown)
                AddLockedDownProperty(list);

            Mobile blessedFor = this.BlessedFor;

            if (blessedFor != null && !blessedFor.Deleted)
                AddBlessedForProperty(list, blessedFor);

            if (DisplayLootType)
                AddLootTypeProperty(list);

            if (DisplayWeight)
                AddWeightProperty(list);

            if (QuestItem)
                AddQuestItemProperty(list);


            AppendChildNameProperties(list);
        }

        /// <summary>
        /// Overridable. Adds the "Quest Item" property to the given <see cref="ObjectPropertyList" />.
        /// </summary>
        public virtual void AddQuestItemProperty(ObjectPropertyList list)
        {
            list.Add(1072351); // Quest Item
        }

        /// <summary>
        /// Overridable. Adds the "Locked Down & Secure" property to the given <see cref="ObjectPropertyList" />.
        /// </summary>
        public virtual void AddSecureProperty(ObjectPropertyList list)
        {
            list.Add(501644); // locked down & secure
        }

        /// <summary>
        /// Overridable. Adds the "Locked Down" property to the given <see cref="ObjectPropertyList" />.
        /// </summary>
        public virtual void AddLockedDownProperty(ObjectPropertyList list)
        {
            list.Add(501643); // locked down
        }

        /// <summary>
        /// Overridable. Adds the "Blessed for ~1_NAME~" property to the given <see cref="ObjectPropertyList" />.
        /// </summary>
        public virtual void AddBlessedForProperty(ObjectPropertyList list, Mobile m)
        {
            list.Add(1062203, "{0}", m.Name); // Blessed for ~1_NAME~
        }

        /// <summary>
        /// Overridable. Fills an <see cref="ObjectPropertyList" /> with everything applicable. By default, this invokes <see cref="AddNameProperties" />, then <see cref="Item.GetChildProperties">Item.GetChildProperties</see> or <see cref="Mobile.GetChildProperties">Mobile.GetChildProperties</see>. This method should be overriden to add any custom properties.
        /// </summary>
        public virtual void GetProperties(ObjectPropertyList list)
        {
            AddNameProperties(list);
        }

        /// <summary>
        /// Overridable. Event invoked when a child (<paramref name="item" />) is building it's <see cref="ObjectPropertyList" />. Recursively calls <see cref="Item.GetChildProperties">Item.GetChildProperties</see> or <see cref="Mobile.GetChildProperties">Mobile.GetChildProperties</see>.
        /// </summary>
        public virtual void GetChildProperties(ObjectPropertyList list, Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).GetChildProperties(list, item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).GetChildProperties(list, item);
        }

        /// <summary>
        /// Overridable. Event invoked when a child (<paramref name="item" />) is building it's Name <see cref="ObjectPropertyList" />. Recursively calls <see cref="Item.GetChildProperties">Item.GetChildNameProperties</see> or <see cref="Mobile.GetChildProperties">Mobile.GetChildNameProperties</see>.
        /// </summary>
        public virtual void GetChildNameProperties(ObjectPropertyList list, Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).GetChildNameProperties(list, item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).GetChildNameProperties(list, item);
        }

        public virtual bool IsChildVisibleTo(Mobile m, Item child)
        {
            return true;
        }

        public void Bounce(Mobile from)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).RemoveItem(this);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).RemoveItem(this);

            m_Parent = null;

            BounceInfo bounce = this.GetBounce();

            if (bounce != null)
            {
                object parent = bounce.m_Parent;

                if (parent is Item && !((Item)parent).Deleted)
                {
                    Item p = (Item)parent;
                    object root = p.RootParent;
                    if (p.IsAccessibleTo(from) && (!(root is Mobile) || ((Mobile)root).CheckNonlocalDrop(from, this, p)))
                    {
                        Location = bounce.m_Location;
                        p.AddItem(this);
                    }
                    else
                    {
                        MoveToWorld(from.Location, from.Map);
                    }
                }
                else if (parent is Mobile && !((Mobile)parent).Deleted)
                {
                    if (!((Mobile)parent).EquipItem(this))
                        MoveToWorld(bounce.m_WorldLoc, bounce.m_Map);
                }
                else
                {
                    MoveToWorld(bounce.m_WorldLoc, bounce.m_Map);
                }

                ClearBounce();
            }
            else
            {
                MoveToWorld(from.Location, from.Map);
            }
        }

        /// <summary>
        /// Overridable. Method checked to see if this item may be equiped while casting a spell. By default, this returns false. It is overriden on spellbook and spell channeling weapons or shields.
        /// </summary>
        /// <returns>True if it may, false if not.</returns>
        /// <example>
        /// <code>
        ///	public override bool AllowEquipedCast( Mobile from )
        ///	{
        ///		if ( from.Int &gt;= 100 )
        ///			return true;
        ///		
        ///		return base.AllowEquipedCast( from );
        /// }</code>
        /// 
        /// When placed in an Item script, the item may be cast when equiped if the <paramref name="from" /> has 100 or more intelligence. Otherwise, it will drop to their backpack.
        /// </example>
        public virtual bool AllowEquipedCast(Mobile from)
        {
            return false;
        }

        public virtual bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            return (m_Layer == layer);
        }

        public virtual bool CanEquip(Mobile m)
        {
            return (m_Layer != Layer.Invalid && m.FindItemOnLayer(m_Layer) == null);
        }

        public virtual void GetChildContextMenuEntries(Mobile from, List<ContextMenuEntry> list, Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).GetChildContextMenuEntries(from, list, item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).GetChildContextMenuEntries(from, list, item);
        }

        public virtual void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).GetChildContextMenuEntries(from, list, this);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).GetChildContextMenuEntries(from, list, this);
        }

        public virtual bool VerifyMove(Mobile from)
        {
            return Movable;
        }

        public virtual DeathMoveResult OnParentDeath(Mobile parent)
        {
            if (!Movable)
                return DeathMoveResult.RemainEquiped;
            else if (m_DonationItem)
                return DeathMoveResult.MoveToBackpack;
            else if (parent.KeepsItemsOnDeath)
                return DeathMoveResult.MoveToBackpack;
            else if (CheckBlessed(parent, true))
                return DeathMoveResult.MoveToBackpack;
            else if (CheckNewbied() && parent.ShortTermMurders < 5)
                return DeathMoveResult.MoveToBackpack;
            else if (parent.Player && Nontransferable)
                return DeathMoveResult.MoveToBackpack;
            else
                return DeathMoveResult.MoveToCorpse;
        }

        public virtual DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            if (!Movable)
                return DeathMoveResult.MoveToBackpack;
            else if (m_DonationItem)
                return DeathMoveResult.MoveToBackpack;
            else if (parent.KeepsItemsOnDeath)
                return DeathMoveResult.MoveToBackpack;
            else if (CheckBlessed(parent, true))
                return DeathMoveResult.MoveToBackpack;
            else if (CheckNewbied() && parent.ShortTermMurders < 5)
                return DeathMoveResult.MoveToBackpack;
            else if (parent.Player && Nontransferable)
                return DeathMoveResult.MoveToBackpack;
            else
                return DeathMoveResult.MoveToCorpse;
        }

        /// <summary>
        /// Moves the Item to <paramref name="location" />. The Item does not change maps.
        /// </summary>
        public virtual void MoveToWorld(Point3D location)
        {
            MoveToWorld(location, m_Map);
        }

        public void LabelTo(Mobile to, int number)
        {
            to.Send(new MessageLocalized(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", ""));
        }

        public void LabelTo(Mobile to, int number, string args)
        {
            to.Send(new MessageLocalized(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", args));
        }

        public void LabelTo(Mobile to, string text)
        {
            to.Send(new UnicodeMessage(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", text));
        }

        public void LabelTo(Mobile to, string format, params object[] args)
        {
            LabelTo(to, String.Format(format, args));
        }

        public void LabelToAffix(Mobile to, int number, AffixType type, string affix)
        {
            to.Send(new MessageLocalizedAffix(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", type, affix, ""));
        }

        public void LabelToAffix(Mobile to, int number, AffixType type, string affix, string args)
        {
            to.Send(new MessageLocalizedAffix(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, number, "", type, affix, args));
        }

        public virtual void LabelLootTypeTo(Mobile to)
        {
            if (m_LootType == LootType.Blessed)
                LabelTo(to, 1041362); // (blessed)

            else if (m_LootType == LootType.Cursed)
                LabelTo(to, "(cursed)");
        }

        public bool AtWorldPoint(int x, int y)
        {
            return (m_Parent == null && m_Location.m_X == x && m_Location.m_Y == y);
        }

        public bool AtPoint(int x, int y)
        {
            return (m_Location.m_X == x && m_Location.m_Y == y);
        }

        /// <summary>
        /// Moves the Item to a given <paramref name="location" /> and <paramref name="map" />.
        /// </summary>
        public void MoveToWorld(Point3D location, Map map)
        {
            if (Deleted)
                return;

            Point3D oldLocation = GetWorldLocation();
            Point3D oldRealLocation = m_Location;

            SetLastMoved();

            if (Parent is Mobile)
                ((Mobile)Parent).RemoveItem(this);
            else if (Parent is Item)
                ((Item)Parent).RemoveItem(this);

            if (m_Map != map)
            {
                Map old = m_Map;

                if (m_Map != null)
                {
                    m_Map.OnLeave(this);

                    if (oldLocation.m_X != 0)
                    {
                        IPooledEnumerable<NetState> eable = m_Map.GetClientsInRange(oldLocation, GetMaxUpdateRange());

                        foreach (NetState state in eable)
                        {
                            Mobile m = state.Mobile;

                            if (m.InRange(oldLocation, GetUpdateRange(m)))
                            {
                                state.Send(this.RemovePacket);
                            }
                        }

                        eable.Free();
                    }
                }

                m_Location = location;
                this.OnLocationChange(oldRealLocation);

                ReleaseWorldPackets();

                List<Item> items = LookupItems();

                if (items != null)
                {
                    for (int i = 0; i < items.Count; ++i)
                        items[i].Map = map;
                }

                m_Map = map;

                if (m_Map != null)
                    m_Map.OnEnter(this);

                OnMapChange();

                if (m_Map != null)
                {
                    IPooledEnumerable<NetState> eable = m_Map.GetClientsInRange(m_Location, GetMaxUpdateRange());

                    foreach (NetState state in eable)
                    {
                        Mobile m = state.Mobile;

                        if (m.CanSee(this) && m.InRange(m_Location, GetUpdateRange(m)))
                            SendInfoTo(state);
                    }

                    eable.Free();
                }

                RemDelta(ItemDelta.Update);

                if (old == null || old == Map.Internal)
                    InvalidateProperties();
            }
            else if (m_Map != null)
            {
                IPooledEnumerable<NetState> eable;

                if (oldLocation.m_X != 0)
                {
                    eable = m_Map.GetClientsInRange(oldLocation, GetMaxUpdateRange());

                    foreach (NetState state in eable)
                    {
                        Mobile m = state.Mobile;

                        if (!m.InRange(location, GetUpdateRange(m)))
                        {
                            state.Send(this.RemovePacket);
                        }
                    }

                    eable.Free();
                }

                Point3D oldInternalLocation = m_Location;

                m_Location = location;
                this.OnLocationChange(oldRealLocation);

                ReleaseWorldPackets();

                eable = m_Map.GetClientsInRange(m_Location, GetMaxUpdateRange());

                foreach (NetState state in eable)
                {
                    Mobile m = state.Mobile;

                    if (m.CanSee(this) && m.InRange(m_Location, GetUpdateRange(m)))
                        SendInfoTo(state);
                }

                eable.Free();

                m_Map.OnMove(oldInternalLocation, this);

                RemDelta(ItemDelta.Update);
            }
            else
            {
                Map = map;
                Location = location;
            }
        }

        /// <summary>
        /// Has the item been deleted?
        /// </summary>
        public bool Deleted { get { return GetFlag(ImplFlag.Deleted); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public LootType LootType
        {
            get
            {
                return m_LootType;
            }
            set
            {
                if (m_LootType != value)
                {
                    m_LootType = value;

                    if (DisplayLootType)
                        InvalidateProperties();
                }
            }
        }

        private static TimeSpan m_DDT = TimeSpan.FromHours(1.0);

        public static TimeSpan DefaultDecayTime { get { return m_DDT; } set { m_DDT = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan DecayTime
        {
            get
            {
                return m_DDT;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Decays
        {
            get
            {
                // TODO: Make item decay an option on the spawner
                return (Movable && Visible/* && Spawner == null*/);
            }
        }

        public virtual bool OnDecay()
        {
            return (Decays && Parent == null && Map != Map.Internal && Region.Find(Location, Map).OnDecay(this));
        }

        public void SetLastMoved()
        {
            m_LastMovedTime = DateTime.UtcNow;
        }

        public DateTime LastMoved
        {
            get
            {
                return m_LastMovedTime;
            }
            set
            {
                m_LastMovedTime = value;
            }
        }

        public bool StackWith(Mobile from, Item dropped)
        {
            return StackWith(from, dropped, true);
        }

        public virtual int MaxStack
        {
            get { return 60000; }
        }

        public virtual bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped.Movable && (Movable || (!Movable && IsLockedDown)) && dropped.Stackable && Stackable && dropped.GetType() == GetType() && dropped.ItemID == ItemID && dropped.Hue == Hue && dropped.Name == Name && (dropped.Amount + Amount) <= MaxStack && dropped != this && !dropped.Nontransferable && !Nontransferable)
            {
                if (m_LootType != dropped.m_LootType)
                    m_LootType = LootType.Regular;

                Amount += dropped.Amount;
                dropped.Delete();

                if (playSound && from != null)
                {
                    int soundID = GetDropSound();

                    if (soundID == -1)
                        soundID = 0x42;

                    from.SendSound(soundID, GetWorldLocation());
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Overrideable function for hiding specific items from players.
        /// </summary>
        public virtual bool CanBeSeenBy(Mobile from)
        {
            return true;
        }

        public virtual bool OnDragDrop(Mobile from, Item dropped)
        {
            if (Parent is Container)
                return ((Container)Parent).OnStackAttempt(from, this, dropped);

            return StackWith(from, dropped);
        }

        public Rectangle2D GetGraphicBounds()
        {
            int itemID = m_ItemID;
            bool doubled = m_Amount > 1;

            if (itemID >= 0xEEA && itemID <= 0xEF2) // Are we coins?
            {
                int coinBase = (itemID - 0xEEA) / 3;
                coinBase *= 3;
                coinBase += 0xEEA;

                doubled = false;

                if (m_Amount <= 1)
                {
                    // A single coin
                    itemID = coinBase;
                }
                else if (m_Amount <= 5)
                {
                    // A stack of coins
                    itemID = coinBase + 1;
                }
                else // m_Amount > 5
                {
                    // A pile of coins
                    itemID = coinBase + 2;
                }
            }

            Rectangle2D bounds = ItemBounds.Table[itemID & 0x3FFF];

            if (doubled)
            {
                bounds.Set(bounds.X, bounds.Y, bounds.Width + 5, bounds.Height + 5);
            }

            return bounds;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Stackable
        {
            get { return GetFlag(ImplFlag.Stackable); }
            set { SetFlag(ImplFlag.Stackable, value); }
        }

        private object _rpl = new object();

        public Packet RemovePacket
        {
            get
            {
                if (m_RemovePacket == null)
                {
                    lock (_rpl)
                    {
                        if (m_RemovePacket == null)
                        {
                            m_RemovePacket = new RemoveItem(this);
                            m_RemovePacket.SetStatic();
                        }
                    }
                }

                return m_RemovePacket;
            }
        }

        private object _opll = new object();

        public Packet OPLPacket
        {
            get
            {
                if (m_OPLPacket == null)
                {
                    lock (_opll)
                    {
                        if (m_OPLPacket == null)
                        {
                            m_OPLPacket = new OPLInfo(PropertyList);
                            m_OPLPacket.SetStatic();
                        }
                    }
                }

                return m_OPLPacket;
            }
        }

        public ObjectPropertyList PropertyList
        {
            get
            {
                if (m_PropertyList == null)
                {
                    m_PropertyList = new ObjectPropertyList(this);

                    GetProperties(m_PropertyList);
                    AppendChildProperties(m_PropertyList);

                    m_PropertyList.Terminate();
                    m_PropertyList.SetStatic();
                }

                return m_PropertyList;
            }
        }

        public virtual void AppendChildProperties(ObjectPropertyList list)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).GetChildProperties(list, this);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).GetChildProperties(list, this);
        }

        public virtual void AppendChildNameProperties(ObjectPropertyList list)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).GetChildNameProperties(list, this);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).GetChildNameProperties(list, this);
        }

        public void ClearProperties()
        {
            Packet.Release(ref m_PropertyList);
            Packet.Release(ref m_OPLPacket);
        }

        public void InvalidateProperties()
        {
            if (!ObjectPropertyList.Enabled)
                return;

            if (m_Map != null && m_Map != Map.Internal && !World.Loading)
            {
                ObjectPropertyList oldList = m_PropertyList;
                m_PropertyList = null;
                ObjectPropertyList newList = PropertyList;

                if (oldList == null || oldList.Hash != newList.Hash)
                {
                    Packet.Release(ref m_OPLPacket);
                    Delta(ItemDelta.Properties);
                }
            }

            else
                ClearProperties();
        }

        private object _wpl = new object();
        private object _wplsa = new object();
        private object _wplhs = new object();

        public Packet WorldPacket
        {
            get
            {
                // This needs to be invalidated when any of the following changes:
                //  - ItemID
                //  - Amount
                //  - Location
                //  - Hue
                //  - Packet Flags
                //  - Direction

                if (m_WorldPacket == null)
                {
                    lock (_wpl)
                    {
                        if (m_WorldPacket == null)
                        {
                            m_WorldPacket = new WorldItem(this);
                            m_WorldPacket.SetStatic();
                        }
                    }
                }

                return m_WorldPacket;
            }
        }

        public Packet WorldPacketSA
        {
            get
            {
                // This needs to be invalidated when any of the following changes:
                //  - ItemID
                //  - Amount
                //  - Location
                //  - Hue
                //  - Packet Flags
                //  - Direction

                if (m_WorldPacketSA == null)
                {
                    lock (_wplsa)
                    {
                        if (m_WorldPacketSA == null)
                        {
                            m_WorldPacketSA = new WorldItemSA(this);
                            m_WorldPacketSA.SetStatic();
                        }
                    }
                }

                return m_WorldPacketSA;
            }
        }

        public Packet WorldPacketHS
        {
            get
            {
                // This needs to be invalidated when any of the following changes:
                //  - ItemID
                //  - Amount
                //  - Location
                //  - Hue
                //  - Packet Flags
                //  - Direction

                if (m_WorldPacketHS == null)
                {
                    lock (_wplhs)
                    {
                        if (m_WorldPacketHS == null)
                        {
                            m_WorldPacketHS = new WorldItemHS(this);
                            m_WorldPacketHS.SetStatic();
                        }
                    }
                }

                return m_WorldPacketHS;
            }
        }

        public void ReleaseWorldPackets()
        {
            Packet.Release(ref m_WorldPacket);
            Packet.Release(ref m_WorldPacketSA);
            Packet.Release(ref m_WorldPacketHS);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Visible
        {
            get
            {
                return GetFlag(ImplFlag.Visible);
            }
            set
            {
                if (GetFlag(ImplFlag.Visible) != value)
                {
                    SetFlag(ImplFlag.Visible, value);
                    ReleaseWorldPackets();

                    if (m_Map != null)
                    {
                        Point3D worldLoc = GetWorldLocation();

                        IPooledEnumerable<NetState> eable = m_Map.GetClientsInRange(worldLoc, GetMaxUpdateRange());

                        foreach (NetState state in eable)
                        {
                            Mobile m = state.Mobile;

                            if (!m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
                            {
                                state.Send(this.RemovePacket);
                            }
                        }

                        eable.Free();
                    }

                    Delta(ItemDelta.Update);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Movable
        {
            get { return GetFlag(ImplFlag.Movable); }
            set
            {
                if (GetFlag(ImplFlag.Movable) != value)
                {
                    SetFlag(ImplFlag.Movable, value);
                    ReleaseWorldPackets();
                    Delta(ItemDelta.Update);
                }
            }
        }

        public virtual bool ForceShowProperties { get { return false; } }

        public virtual int GetPacketFlags()
        {
            int flags = 0;

            if (!Visible)
                flags |= 0x80;

            if (Movable || ForceShowProperties)
                flags |= 0x20;

            return flags;
        }

        public virtual bool OnMoveOff(Mobile m)
        {
            return true;
        }

        public virtual bool OnMoveOver(Mobile m)
        {
            return true;
        }

        public virtual bool HandlesOnMovement { get { return false; } }

        public virtual void OnMovement(Mobile m, Point3D oldLocation)
        {
        }

        public void Internalize()
        {
            MoveToWorld(Point3D.Zero, Map.Internal);
        }

        public virtual void OnMapChange()
        {
        }

        public virtual void OnRemoved(object parent)
        {
        }

        public virtual void OnAdded(object parent)
        {
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Map Map
        {
            get
            {
                return m_Map;
            }
            set
            {
                if (m_Map != value)
                {
                    Map old = m_Map;

                    if (m_Map != null && m_Parent == null)
                    {
                        m_Map.OnLeave(this);
                        SendRemovePacket();
                    }

                    List<Item> items = LookupItems();

                    if (items != null)
                    {
                        for (int i = 0; i < items.Count; ++i)
                            items[i].Map = value;
                    }

                    m_Map = value;

                    if (m_Map != null && m_Parent == null)
                        m_Map.OnEnter(this);

                    Delta(ItemDelta.Update);

                    this.OnMapChange();

                    if (old == null || old == Map.Internal)
                        InvalidateProperties();
                }
            }
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Direction = 0x00000001,
            Bounce = 0x00000002,
            LootType = 0x00000004,
            LocationFull = 0x00000008,
            ItemID = 0x00000010,
            Hue = 0x00000020,
            Amount = 0x00000040,
            Layer = 0x00000080,
            Name = 0x00000100,
            Parent = 0x00000200,
            Items = 0x00000400,
            WeightNot1or0 = 0x00000800,
            Map = 0x00001000,
            Visible = 0x00002000,
            Movable = 0x00004000,
            Stackable = 0x00008000,
            WeightIs0 = 0x00010000,
            LocationSByteZ = 0x00020000,
            LocationShortXY = 0x00040000,
            LocationByteXY = 0x00080000,
            ImplFlags = 0x00100000,
            InsuredFor = 0x00200000,
            BlessedFor = 0x00400000,
            HeldBy = 0x00800000,
            IntWeight = 0x01000000,
            SavedFlags = 0x02000000,
            NullWeight = 0x04000000,
            DonationItem = 0x08000000
        }

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        int ISerializable.TypeReference
        {
            get { return m_TypeRef; }
        }

        int ISerializable.SerialIdentity
        {
            get { return m_Serial; }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(18); // version

            //Version 18
            writer.Write((int)m_ItemGroup);
            writer.Write((int)m_ItemRarity);
            writer.Write(m_Identified);
            writer.Write((int)m_Dungeon);
            writer.Write(m_ArcaneRechargable);
            writer.Write(m_ArcaneCharges);
            writer.Write(m_ArcaneChargesMax);
            writer.Write(m_TierLevel);
            writer.Write(m_Experience);
            writer.Write((int)m_Resource);
            writer.Write(m_DisplayCrafter);
            writer.Write((int)m_Quality);
            writer.Write(m_DecorativeEquipment);
            writer.Write(m_CraftedBy);
            writer.Write(m_CrafterName);
            writer.Write(m_Stealable);
            writer.Write(m_AlreadyStolen);
            writer.Write(m_MinimumStealing);
            writer.Write(m_MaximumStealing);
            writer.Write(m_OceanStatic);
            writer.Write(m_ShipItem);
            writer.Write((int)m_PlayerClass);
            writer.Write(m_PlayerClassRestricted);
            writer.Write(m_PlayerClassOwner);
            writer.Write((int)BreakChanceFromReforge);
            writer.Write((byte)Acquisition);
            writer.Write(AcquisitionData);

            SaveFlag flags = SaveFlag.None;

            int x = m_Location.m_X, y = m_Location.m_Y, z = m_Location.m_Z;

            if (x != 0 || y != 0 || z != 0)
            {
                if (x >= short.MinValue && x <= short.MaxValue && y >= short.MinValue && y <= short.MaxValue && z >= sbyte.MinValue && z <= sbyte.MaxValue)
                {
                    if (x != 0 || y != 0)
                    {
                        if (x >= byte.MinValue && x <= byte.MaxValue && y >= byte.MinValue && y <= byte.MaxValue)
                            flags |= SaveFlag.LocationByteXY;
                        else
                            flags |= SaveFlag.LocationShortXY;
                    }

                    if (z != 0)
                        flags |= SaveFlag.LocationSByteZ;
                }
                else
                {
                    flags |= SaveFlag.LocationFull;
                }
            }

            CompactInfo info = LookupCompactInfo();
            List<Item> items = LookupItems();

            if (m_Direction != Direction.North)
                flags |= SaveFlag.Direction;
            if (info != null && info.m_Bounce != null)
                flags |= SaveFlag.Bounce;
            if (m_LootType != LootType.Regular)
                flags |= SaveFlag.LootType;
            if (m_ItemID != 0)
                flags |= SaveFlag.ItemID;
            if (m_Hue != 0)
                flags |= SaveFlag.Hue;
            if (m_Amount != 1)
                flags |= SaveFlag.Amount;
            if (m_Layer != Layer.Invalid)
                flags |= SaveFlag.Layer;
            if (info != null && info.m_Name != null)
                flags |= SaveFlag.Name;
            if (m_Parent != null)
                flags |= SaveFlag.Parent;
            if (items != null && items.Count > 0)
                flags |= SaveFlag.Items;
            if (m_Map != Map.Internal)
                flags |= SaveFlag.Map;
            //if ( m_InsuredFor != null && !m_InsuredFor.Deleted )
            //flags |= SaveFlag.InsuredFor;
            if (info != null && info.m_BlessedFor != null && !info.m_BlessedFor.Deleted)
                flags |= SaveFlag.BlessedFor;
            if (info != null && info.m_HeldBy != null && !info.m_HeldBy.Deleted)
                flags |= SaveFlag.HeldBy;
            if (info != null && info.m_SavedFlags != 0)
                flags |= SaveFlag.SavedFlags;
            if (m_DonationItem)
                flags |= SaveFlag.DonationItem;

            if (info == null || info.m_Weight == -1)
            {
                flags |= SaveFlag.NullWeight;
            }
            else
            {
                if (info.m_Weight == 0.0)
                {
                    flags |= SaveFlag.WeightIs0;
                }
                else if (info.m_Weight != 1.0)
                {
                    if (info.m_Weight == (int)info.m_Weight)
                        flags |= SaveFlag.IntWeight;
                    else
                        flags |= SaveFlag.WeightNot1or0;
                }
            }

            ImplFlag implFlags = (m_Flags & (ImplFlag.Visible | ImplFlag.Movable | ImplFlag.Stackable | ImplFlag.Insured | ImplFlag.PayedInsurance | ImplFlag.QuestItem));

            if (implFlags != (ImplFlag.Visible | ImplFlag.Movable))
                flags |= SaveFlag.ImplFlags;

            writer.Write((int)flags);

            /* begin last moved time optimization */
            long ticks = m_LastMovedTime.Ticks;
            long now = DateTime.UtcNow.Ticks;

            TimeSpan d;

            try { d = new TimeSpan(ticks - now); }
            catch { if (ticks < now) d = TimeSpan.MaxValue; else d = TimeSpan.MaxValue; }

            double minutes = -d.TotalMinutes;

            if (minutes < int.MinValue)
                minutes = int.MinValue;
            else if (minutes > int.MaxValue)
                minutes = int.MaxValue;

            writer.WriteEncodedInt((int)minutes);
            /* end */

            if (GetSaveFlag(flags, SaveFlag.Direction))
                writer.Write((byte)m_Direction);

            if (GetSaveFlag(flags, SaveFlag.Bounce))
                BounceInfo.Serialize(info.m_Bounce, writer);

            if (GetSaveFlag(flags, SaveFlag.LootType))
                writer.Write((byte)m_LootType);

            if (GetSaveFlag(flags, SaveFlag.LocationFull))
            {
                writer.WriteEncodedInt(x);
                writer.WriteEncodedInt(y);
                writer.WriteEncodedInt(z);
            }
            else
            {
                if (GetSaveFlag(flags, SaveFlag.LocationByteXY))
                {
                    writer.Write((byte)x);
                    writer.Write((byte)y);
                }
                else if (GetSaveFlag(flags, SaveFlag.LocationShortXY))
                {
                    writer.Write((short)x);
                    writer.Write((short)y);
                }

                if (GetSaveFlag(flags, SaveFlag.LocationSByteZ))
                    writer.Write((sbyte)z);
            }

            if (GetSaveFlag(flags, SaveFlag.ItemID))
                writer.WriteEncodedInt((int)m_ItemID);

            if (GetSaveFlag(flags, SaveFlag.Hue))
                writer.WriteEncodedInt((int)OriginalHue); // IPY : Changed from writin m_Hue to writing m_OriginalHue as m_Hue can be a temporary arena color

            if (GetSaveFlag(flags, SaveFlag.Amount))
                writer.WriteEncodedInt((int)m_Amount);

            if (GetSaveFlag(flags, SaveFlag.Layer))
                writer.Write((byte)m_Layer);

            if (GetSaveFlag(flags, SaveFlag.Name))
                writer.Write((string)info.m_Name);

            if (GetSaveFlag(flags, SaveFlag.Parent))
            {
                if (m_Parent is Mobile && !((Mobile)m_Parent).Deleted)
                    writer.Write(((Mobile)m_Parent).Serial);
                else if (m_Parent is Item && !((Item)m_Parent).Deleted)
                    writer.Write(((Item)m_Parent).Serial);
                else
                    writer.Write((int)Serial.MinusOne);
            }

            if (GetSaveFlag(flags, SaveFlag.Items))
                writer.Write(items, false);

            if (GetSaveFlag(flags, SaveFlag.IntWeight))
                writer.WriteEncodedInt((int)info.m_Weight);
            else if (GetSaveFlag(flags, SaveFlag.WeightNot1or0))
                writer.Write((double)info.m_Weight);

            if (GetSaveFlag(flags, SaveFlag.Map))
                writer.Write((Map)m_Map);

            if (GetSaveFlag(flags, SaveFlag.ImplFlags))
                writer.WriteEncodedInt((int)implFlags);

            if (GetSaveFlag(flags, SaveFlag.InsuredFor))
                writer.Write((Mobile)null);

            if (GetSaveFlag(flags, SaveFlag.BlessedFor))
                writer.Write(info.m_BlessedFor);

            if (GetSaveFlag(flags, SaveFlag.HeldBy))
                writer.Write(info.m_HeldBy);

            if (GetSaveFlag(flags, SaveFlag.SavedFlags))
                writer.WriteEncodedInt(info.m_SavedFlags);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            SetLastMoved();

            switch (version)
            {
                case 18:
                    {
                        m_ItemGroup = (ItemGroupType)reader.ReadInt();
                        m_ItemRarity = (ItemRarityType)reader.ReadInt();

                        m_Identified = reader.ReadBool();
                        m_Dungeon = (DungeonEnum)reader.ReadInt();
                        m_ArcaneRechargable = reader.ReadBool();
                        m_ArcaneCharges = reader.ReadInt();
                        m_ArcaneChargesMax = reader.ReadInt();
                        m_TierLevel = reader.ReadInt();
                        m_Experience = reader.ReadInt();

                        m_Resource = (CraftResource)reader.ReadInt();
                        m_DisplayCrafter = reader.ReadBool();
                        m_Quality = (Quality)reader.ReadInt();
                        m_DecorativeEquipment = reader.ReadBool();

                        goto case 17;
                    }

                case 17:
                    {
                        m_CraftedBy = reader.ReadMobile();
                        m_CrafterName = reader.ReadString();

                        goto case 16;
                    }

                case 16:
                    {
                        goto case 15;
                    }
                case 15:
                    {
                        m_Stealable = reader.ReadBool();
                        m_AlreadyStolen = reader.ReadBool();
                        m_MinimumStealing = reader.ReadDouble();
                        m_MaximumStealing = reader.ReadDouble();

                        goto case 14;
                    }
                case 14:
                    {
                        m_OceanStatic = reader.ReadBool();
                        m_ShipItem = reader.ReadBool();

                        goto case 13;
                    }

                case 13:
                    {
                        m_PlayerClass = (PlayerClass)reader.ReadInt();
                        m_PlayerClassRestricted = reader.ReadBool();
                        m_PlayerClassOwner = reader.ReadMobile();

                        goto case 12;
                    }

                case 12:
                    {
                        BreakChanceFromReforge = reader.ReadInt();
                        goto case 11;
                    }
                case 11:
                    {
                        BreakChanceFromReforge = 5;
                        Acquisition = (AcquisitionType)reader.ReadByte();
                        goto case 10;
                    }
                case 10:
                    {
                        // IPY
                        // legacy compatibility, arena hues used to be serialized. No longer. (HIJACKED THIS)
                        AcquisitionData = reader.ReadInt();
                        OriginalHue = 0;
                        goto case 9;

                    }
                case 9:
                case 8:
                case 7:
                case 6:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadInt();

                        if (version < 7)
                        {
                            LastMoved = reader.ReadDeltaTime();
                        }
                        else
                        {
                            int minutes = reader.ReadEncodedInt();

                            try { LastMoved = DateTime.UtcNow - TimeSpan.FromMinutes(minutes); }
                            catch { LastMoved = DateTime.UtcNow; }
                        }

                        if (GetSaveFlag(flags, SaveFlag.DonationItem))
                            m_DonationItem = true;

                        if (GetSaveFlag(flags, SaveFlag.Direction))
                            m_Direction = (Direction)reader.ReadByte();

                        if (GetSaveFlag(flags, SaveFlag.Bounce))
                            AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize(reader);

                        if (GetSaveFlag(flags, SaveFlag.LootType))
                            m_LootType = (LootType)reader.ReadByte();

                        int x = 0, y = 0, z = 0;

                        if (GetSaveFlag(flags, SaveFlag.LocationFull))
                        {
                            x = reader.ReadEncodedInt();
                            y = reader.ReadEncodedInt();
                            z = reader.ReadEncodedInt();
                        }
                        else
                        {
                            if (GetSaveFlag(flags, SaveFlag.LocationByteXY))
                            {
                                x = reader.ReadByte();
                                y = reader.ReadByte();
                            }
                            else if (GetSaveFlag(flags, SaveFlag.LocationShortXY))
                            {
                                x = reader.ReadShort();
                                y = reader.ReadShort();
                            }

                            if (GetSaveFlag(flags, SaveFlag.LocationSByteZ))
                                z = reader.ReadSByte();
                        }

                        m_Location = new Point3D(x, y, z);

                        if (GetSaveFlag(flags, SaveFlag.ItemID))
                            m_ItemID = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Hue))
                        {
                            m_Hue = reader.ReadEncodedInt();
                            OriginalHue = m_Hue; // IPY : Set the OriginalHue on startup.
                        }
                        if (GetSaveFlag(flags, SaveFlag.Amount))
                            m_Amount = reader.ReadEncodedInt();
                        else
                            m_Amount = 1;

                        if (GetSaveFlag(flags, SaveFlag.Layer))
                            m_Layer = (Layer)reader.ReadByte();

                        if (GetSaveFlag(flags, SaveFlag.Name))
                        {
                            string name = reader.ReadString();

                            if (name != this.DefaultName)
                                AcquireCompactInfo().m_Name = name;
                        }

                        if (GetSaveFlag(flags, SaveFlag.Parent))
                        {
                            Serial parent = reader.ReadInt();

                            if (parent.IsMobile)
                                m_Parent = World.FindMobile(parent);
                            else if (parent.IsItem)
                                m_Parent = World.FindItem(parent);
                            else
                                m_Parent = null;

                            if (m_Parent == null && (parent.IsMobile || parent.IsItem))
                                Delete();
                        }

                        if (GetSaveFlag(flags, SaveFlag.Items))
                        {
                            List<Item> items = reader.ReadStrongItemList();

                            if (this is Container)
                                (this as Container).m_Items = items;
                            else
                                AcquireCompactInfo().m_Items = items;
                        }

                        if (version < 8 || !GetSaveFlag(flags, SaveFlag.NullWeight))
                        {
                            double weight;

                            if (GetSaveFlag(flags, SaveFlag.IntWeight))
                                weight = reader.ReadEncodedInt();
                            else if (GetSaveFlag(flags, SaveFlag.WeightNot1or0))
                                weight = reader.ReadDouble();
                            else if (GetSaveFlag(flags, SaveFlag.WeightIs0))
                                weight = 0.0;
                            else
                                weight = 1.0;

                            if (weight != DefaultWeight)
                                AcquireCompactInfo().m_Weight = weight;
                        }

                        if (GetSaveFlag(flags, SaveFlag.Map))
                            m_Map = reader.ReadMap();
                        else
                            m_Map = Map.Internal;

                        if (GetSaveFlag(flags, SaveFlag.Visible))
                            SetFlag(ImplFlag.Visible, reader.ReadBool());
                        else
                            SetFlag(ImplFlag.Visible, true);

                        if (GetSaveFlag(flags, SaveFlag.Movable))
                            SetFlag(ImplFlag.Movable, reader.ReadBool());
                        else
                            SetFlag(ImplFlag.Movable, true);

                        if (GetSaveFlag(flags, SaveFlag.Stackable))
                            SetFlag(ImplFlag.Stackable, reader.ReadBool());

                        if (GetSaveFlag(flags, SaveFlag.ImplFlags))
                            m_Flags = (ImplFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.InsuredFor))
                            /*m_InsuredFor = */
                            reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.BlessedFor))
                            AcquireCompactInfo().m_BlessedFor = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.HeldBy))
                            AcquireCompactInfo().m_HeldBy = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.SavedFlags))
                            AcquireCompactInfo().m_SavedFlags = reader.ReadEncodedInt();

                        if (m_Map != null && m_Parent == null)
                            m_Map.OnEnter(this);

                        break;
                    }
                case 5:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadInt();

                        LastMoved = reader.ReadDeltaTime();

                        if (GetSaveFlag(flags, SaveFlag.Direction))
                            m_Direction = (Direction)reader.ReadByte();

                        if (GetSaveFlag(flags, SaveFlag.Bounce))
                            AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize(reader);

                        if (GetSaveFlag(flags, SaveFlag.LootType))
                            m_LootType = (LootType)reader.ReadByte();

                        if (GetSaveFlag(flags, SaveFlag.LocationFull))
                            m_Location = reader.ReadPoint3D();

                        if (GetSaveFlag(flags, SaveFlag.ItemID))
                            m_ItemID = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Hue))
                            m_Hue = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Amount))
                            m_Amount = reader.ReadInt();
                        else
                            m_Amount = 1;

                        if (GetSaveFlag(flags, SaveFlag.Layer))
                            m_Layer = (Layer)reader.ReadByte();

                        if (GetSaveFlag(flags, SaveFlag.Name))
                        {
                            string name = reader.ReadString();

                            if (name != this.DefaultName)
                                AcquireCompactInfo().m_Name = name;
                        }

                        if (GetSaveFlag(flags, SaveFlag.Parent))
                        {
                            Serial parent = reader.ReadInt();

                            if (parent.IsMobile)
                                m_Parent = World.FindMobile(parent);
                            else if (parent.IsItem)
                                m_Parent = World.FindItem(parent);
                            else
                                m_Parent = null;

                            if (m_Parent == null && (parent.IsMobile || parent.IsItem))
                                Delete();
                        }

                        if (GetSaveFlag(flags, SaveFlag.Items))
                        {
                            List<Item> items = reader.ReadStrongItemList();

                            if (this is Container)
                                (this as Container).m_Items = items;
                            else
                                AcquireCompactInfo().m_Items = items;
                        }

                        double weight;

                        if (GetSaveFlag(flags, SaveFlag.IntWeight))
                            weight = reader.ReadEncodedInt();
                        else if (GetSaveFlag(flags, SaveFlag.WeightNot1or0))
                            weight = reader.ReadDouble();
                        else if (GetSaveFlag(flags, SaveFlag.WeightIs0))
                            weight = 0.0;
                        else
                            weight = 1.0;

                        if (weight != DefaultWeight)
                            AcquireCompactInfo().m_Weight = weight;

                        if (GetSaveFlag(flags, SaveFlag.Map))
                            m_Map = reader.ReadMap();
                        else
                            m_Map = Map.Internal;

                        if (GetSaveFlag(flags, SaveFlag.Visible))
                            SetFlag(ImplFlag.Visible, reader.ReadBool());
                        else
                            SetFlag(ImplFlag.Visible, true);

                        if (GetSaveFlag(flags, SaveFlag.Movable))
                            SetFlag(ImplFlag.Movable, reader.ReadBool());
                        else
                            SetFlag(ImplFlag.Movable, true);

                        if (GetSaveFlag(flags, SaveFlag.Stackable))
                            SetFlag(ImplFlag.Stackable, reader.ReadBool());

                        if (m_Map != null && m_Parent == null)
                            m_Map.OnEnter(this);

                        break;
                    }
                case 4: // Just removed variables
                case 3:
                    {
                        m_Direction = (Direction)reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        AcquireCompactInfo().m_Bounce = BounceInfo.Deserialize(reader);
                        LastMoved = reader.ReadDeltaTime();

                        goto case 1;
                    }
                case 1:
                    {
                        m_LootType = (LootType)reader.ReadByte();//m_Newbied = reader.ReadBool();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Location = reader.ReadPoint3D();
                        m_ItemID = reader.ReadInt();
                        m_Hue = reader.ReadInt();
                        m_Amount = reader.ReadInt();
                        m_Layer = (Layer)reader.ReadByte();

                        string name = reader.ReadString();

                        if (name != this.DefaultName)
                            AcquireCompactInfo().m_Name = name;

                        Serial parent = reader.ReadInt();

                        if (parent.IsMobile)
                            m_Parent = World.FindMobile(parent);
                        else if (parent.IsItem)
                            m_Parent = World.FindItem(parent);
                        else
                            m_Parent = null;

                        if (m_Parent == null && (parent.IsMobile || parent.IsItem))
                            Delete();

                        int count = reader.ReadInt();

                        if (count > 0)
                        {
                            List<Item> items = new List<Item>(count);

                            for (int i = 0; i < count; ++i)
                            {
                                Item item = reader.ReadItem();

                                if (item != null)
                                    items.Add(item);
                            }

                            if (this is Container)
                                (this as Container).m_Items = items;
                            else
                                AcquireCompactInfo().m_Items = items;
                        }

                        double weight = reader.ReadDouble();

                        if (weight != DefaultWeight)
                            AcquireCompactInfo().m_Weight = weight;

                        if (version <= 3)
                        {
                            reader.ReadInt();
                            reader.ReadInt();
                            reader.ReadInt();
                        }

                        m_Map = reader.ReadMap();
                        SetFlag(ImplFlag.Visible, reader.ReadBool());
                        SetFlag(ImplFlag.Movable, reader.ReadBool());

                        if (version <= 3)
                            /*m_Deleted =*/
                            reader.ReadBool();

                        Stackable = reader.ReadBool();

                        // IPY ARENA SPECIAL HUE
                        OriginalHue = m_Hue;


                        if (m_Map != null && m_Parent == null)
                            m_Map.OnEnter(this);

                        break;
                    }
            }

            if (this.HeldBy != null)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(FixHolding_Sandbox));

            //if ( version < 9 )
            VerifyCompactInfo();
        }

        public IPooledEnumerable<IEntity> GetObjectsInRange(int range)
        {
            Map map = m_Map;

            if (map == null)
                return Server.Map.NullEnumerable<IEntity>.Instance;

            if (m_Parent == null)
                return map.GetObjectsInRange(m_Location, range);

            return map.GetObjectsInRange(GetWorldLocation(), range);
        }

        public IPooledEnumerable<Item> GetItemsInRange(int range)
        {
            Map map = m_Map;

            if (map == null)
                return Server.Map.NullEnumerable<Item>.Instance;

            if (m_Parent == null)
                return map.GetItemsInRange(m_Location, range);

            return map.GetItemsInRange(GetWorldLocation(), range);
        }

        public IPooledEnumerable<Mobile> GetMobilesInRange(int range)
        {
            Map map = m_Map;

            if (map == null)
                return Server.Map.NullEnumerable<Mobile>.Instance;

            if (m_Parent == null)
                return map.GetMobilesInRange(m_Location, range);

            return map.GetMobilesInRange(GetWorldLocation(), range);
        }

        public IPooledEnumerable<NetState> GetClientsInRange(int range)
        {
            Map map = m_Map;

            if (map == null)
                return Server.Map.NullEnumerable<NetState>.Instance;

            if (m_Parent == null)
                return map.GetClientsInRange(m_Location, range);

            return map.GetClientsInRange(GetWorldLocation(), range);
        }

        private static int m_LockedDownFlag;
        private static int m_SecureFlag;

        public static int LockedDownFlag
        {
            get { return m_LockedDownFlag; }
            set { m_LockedDownFlag = value; }
        }

        public static int SecureFlag
        {
            get { return m_SecureFlag; }
            set { m_SecureFlag = value; }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public bool IsLockedDown
        {
            get { return GetTempFlag(m_LockedDownFlag); }
            set { SetTempFlag(m_LockedDownFlag, value); InvalidateProperties(); }
        }

        public bool IsSecure
        {
            get { return GetTempFlag(m_SecureFlag); }
            set { SetTempFlag(m_SecureFlag, value); InvalidateProperties(); }
        }

        public bool GetTempFlag(int flag)
        {
            CompactInfo info = LookupCompactInfo();

            if (info == null)
                return false;

            return ((info.m_TempFlags & flag) != 0);
        }

        public void SetTempFlag(int flag, bool value)
        {
            CompactInfo info = AcquireCompactInfo();

            if (value)
                info.m_TempFlags |= flag;
            else
                info.m_TempFlags &= ~flag;

            if (info.m_TempFlags == 0)
                VerifyCompactInfo();
        }

        public bool GetSavedFlag(int flag)
        {
            CompactInfo info = LookupCompactInfo();

            if (info == null)
                return false;

            return ((info.m_SavedFlags & flag) != 0);
        }

        public void SetSavedFlag(int flag, bool value)
        {
            CompactInfo info = AcquireCompactInfo();

            if (value)
                info.m_SavedFlags |= flag;
            else
                info.m_SavedFlags &= ~flag;

            if (info.m_SavedFlags == 0)
                VerifyCompactInfo();
        }

        private void FixHolding_Sandbox()
        {
            Mobile heldBy = this.HeldBy;

            if (heldBy != null)
            {
                if (this.GetBounce() != null)
                {
                    Bounce(heldBy);
                }
                else
                {
                    heldBy.Holding = null;
                    heldBy.AddToBackpack(this);
                    ClearBounce();
                }
            }
        }

        public virtual int GetMaxUpdateRange()
        {
            return 18;
        }

        public virtual int GetUpdateRange(Mobile m)
        {
            return 18;
        }

        public void SendInfoTo(NetState state)
        {
            SendInfoTo(state, ObjectPropertyList.Enabled);
        }

        public virtual void SendInfoTo(NetState state, bool sendOplPacket)
        {
            state.Send(GetWorldPacketFor(state));

            if (sendOplPacket)
            {
                state.Send(OPLPacket);
            }
        }

        protected virtual Packet GetWorldPacketFor(NetState state)
        {
            if (state == null)
                return this.WorldPacket;

            if (state.HighSeas)
                return this.WorldPacketHS;
            else if (state.StygianAbyss)
                return this.WorldPacketSA;
            else
                return this.WorldPacket;
        }

        public virtual bool IsVirtualItem { get { return false; } }

        public virtual int GetTotal(TotalType type)
        {
            return 0;
        }

        public virtual void UpdateTotal(Item sender, TotalType type, int delta)
        {
            if (!IsVirtualItem)
            {
                if (m_Parent is Item)
                    (m_Parent as Item).UpdateTotal(sender, type, delta);
                else if (m_Parent is Mobile)
                    (m_Parent as Mobile).UpdateTotal(sender, type, delta);
                else if (this.HeldBy != null)
                    (this.HeldBy as Mobile).UpdateTotal(sender, type, delta);
            }
        }

        public virtual void UpdateTotals()
        {
        }

        public virtual int LabelNumber
        {
            get
            {
                if (m_ItemID < 0x4000)
                    return 1020000 + m_ItemID;
                else
                    return 1078872 + m_ItemID;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalGold
        {
            get { return GetTotal(TotalType.Gold); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalItems
        {
            get { return GetTotal(TotalType.Items); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalWeight
        {
            get { return GetTotal(TotalType.Weight); }
        }

        public virtual double DefaultWeight
        {
            get
            {
                if (m_ItemID < 0 || m_ItemID > TileData.MaxItemValue || this is BaseMulti)
                    return 0;

                int weight = TileData.ItemTable[m_ItemID].Weight;

                if (weight == 255 || weight == 0)
                    weight = 1;

                return weight;
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public double Weight
        {
            get
            {
                CompactInfo info = LookupCompactInfo();

                if (info != null && info.m_Weight != -1)
                    return info.m_Weight;

                return this.DefaultWeight;
            }
            set
            {
                if (this.Weight != value)
                {
                    CompactInfo info = AcquireCompactInfo();

                    int oldPileWeight = this.PileWeight;

                    info.m_Weight = value;

                    if (info.m_Weight == -1)
                        VerifyCompactInfo();

                    int newPileWeight = this.PileWeight;

                    UpdateTotal(this, TotalType.Weight, newPileWeight - oldPileWeight);

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int PileWeight
        {
            get
            {
                return (int)Math.Ceiling(this.Weight * this.Amount);
            }
        }

        public virtual int HuedItemID
        {
            get
            {
                return m_ItemID;
            }
        }

        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public virtual int Hue
        {
            get
            {
                return m_Hue;
            }
            set
            {
                OriginalHue = value;
                if (m_Hue != value)
                {
                    m_Hue = value;
                    ReleaseWorldPackets();

                    Delta(ItemDelta.Update);
                }
            }
        }

        public const int QuestItemHue = 0x4EA; // Hmmmm... "for EA"?

        public virtual bool Nontransferable
        {
            get { return QuestItem; }
        }

        public virtual void HandleInvalidTransfer(Mobile from)
        {
            if (QuestItem)
                from.SendLocalizedMessage(1074769); // An item must be in your backpack (and not in a container within) to be toggled as a quest item.
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Layer Layer
        {
            get
            {
                return m_Layer;
            }
            set
            {
                if (m_Layer != value)
                {
                    m_Layer = value;

                    Delta(ItemDelta.EquipOnly);
                }
            }
        }

        public List<Item> Items
        {
            get
            {
                List<Item> items = LookupItems();

                if (items == null)
                    items = EmptyItems;

                return items;
            }
        }

        public object RootParent
        {
            get
            {
                object p = m_Parent;

                while (p is Item)
                {
                    Item item = (Item)p;

                    if (item.m_Parent == null)
                    {
                        break;
                    }
                    else
                    {
                        p = item.m_Parent;
                    }
                }

                return p;
            }
        }

        public bool ParentsContain<T>() where T : Item
        {
            object p = m_Parent;

            while (p is Item)
            {
                if (p is T)
                    return true;

                Item item = (Item)p;

                if (item.m_Parent == null)
                {
                    break;
                }
                else
                {
                    p = item.m_Parent;
                }
            }

            return false;
        }

        public virtual void AddItem(Item item)
        {
            if (item == null || item.Deleted || item.m_Parent == this)
            {
                return;
            }
            else if (item == this)
            {
                Console.WriteLine("Warning: Adding item to itself: [0x{0:X} {1}].AddItem( [0x{2:X} {3}] )", this.Serial.Value, this.GetType().Name, item.Serial.Value, item.GetType().Name);
                Console.WriteLine(new System.Diagnostics.StackTrace());
                return;
            }
            else if (IsChildOf(item))
            {
                Console.WriteLine("Warning: Adding parent item to child: [0x{0:X} {1}].AddItem( [0x{2:X} {3}] )", this.Serial.Value, this.GetType().Name, item.Serial.Value, item.GetType().Name);
                Console.WriteLine(new System.Diagnostics.StackTrace());
                return;
            }
            else if (item.m_Parent is Mobile)
            {
                ((Mobile)item.m_Parent).RemoveItem(item);
            }
            else if (item.m_Parent is Item)
            {
                ((Item)item.m_Parent).RemoveItem(item);
            }
            else
            {
                item.SendRemovePacket();
            }

            item.Parent = this;
            item.Map = m_Map;

            List<Item> items = AcquireItems();

            items.Add(item);

            if (!item.IsVirtualItem)
            {
                UpdateTotal(item, TotalType.Gold, item.TotalGold);
                UpdateTotal(item, TotalType.Items, item.TotalItems + 1);
                UpdateTotal(item, TotalType.Weight, item.TotalWeight + item.PileWeight);
            }

            item.Delta(ItemDelta.Update);

            item.OnAdded(this);
            OnItemAdded(item);
        }

        private static List<Item> m_DeltaQueue = new List<Item>();

        public void Delta(ItemDelta flags)
        {
            if (m_Map == null || m_Map == Map.Internal)
                return;

            m_DeltaFlags |= flags;

            if (!GetFlag(ImplFlag.InQueue))
            {
                SetFlag(ImplFlag.InQueue, true);

                if (_processing)
                {
                    try
                    {
                        using (StreamWriter op = new StreamWriter("delta-recursion.log", true))
                        {
                            op.WriteLine("# {0}", DateTime.UtcNow);
                            op.WriteLine(new System.Diagnostics.StackTrace());
                            op.WriteLine();
                        }
                    }
                    catch { }
                }
                else
                {
                    m_DeltaQueue.Add(this);
                }
            }

            Core.Set();
        }

        public void RemDelta(ItemDelta flags)
        {
            m_DeltaFlags &= ~flags;

            if (GetFlag(ImplFlag.InQueue) && m_DeltaFlags == ItemDelta.None)
            {
                SetFlag(ImplFlag.InQueue, false);

                if (_processing)
                {
                    try
                    {
                        using (StreamWriter op = new StreamWriter("delta-recursion.log", true))
                        {
                            op.WriteLine("# {0}", DateTime.UtcNow);
                            op.WriteLine(new System.Diagnostics.StackTrace());
                            op.WriteLine();
                        }
                    }
                    catch { }
                }
                else
                {
                    m_DeltaQueue.Remove(this);
                }
            }
        }

        private bool m_NoMoveHS;

        public bool NoMoveHS
        {
            get { return m_NoMoveHS; }
            set { m_NoMoveHS = value; }
        }

        public void ProcessDelta()
        {
            ItemDelta flags = m_DeltaFlags;

            SetFlag(ImplFlag.InQueue, false);
            m_DeltaFlags = ItemDelta.None;

            Map map = m_Map;

            if (map != null && !Deleted)
            {
                bool sendOPLUpdate = ObjectPropertyList.Enabled && (flags & ItemDelta.Properties) != 0;

                Container contParent = m_Parent as Container;

                if (contParent != null && !contParent.IsPublicContainer)
                {
                    if ((flags & ItemDelta.Update) != 0)
                    {
                        Point3D worldLoc = GetWorldLocation();

                        Mobile rootParent = contParent.RootParent as Mobile;
                        Mobile tradeRecip = null;

                        if (rootParent != null)
                        {
                            NetState ns = rootParent.NetState;

                            if (ns != null)
                            {
                                if (rootParent.CanSee(this) && rootParent.InRange(worldLoc, GetUpdateRange(rootParent)))
                                {
                                    if (ns.ContainerGridLines)
                                        ns.Send(new ContainerContentUpdate6017(this));
                                    else
                                        ns.Send(new ContainerContentUpdate(this));

                                    if (ObjectPropertyList.Enabled)
                                        ns.Send(OPLPacket);
                                }
                            }
                        }

                        SecureTradeContainer stc = this.GetSecureTradeCont();

                        if (stc != null)
                        {
                            SecureTrade st = stc.Trade;

                            if (st != null)
                            {
                                Mobile test = st.From.Mobile;

                                if (test != null && test != rootParent)
                                    tradeRecip = test;

                                test = st.To.Mobile;

                                if (test != null && test != rootParent)
                                    tradeRecip = test;

                                if (tradeRecip != null)
                                {
                                    NetState ns = tradeRecip.NetState;

                                    if (ns != null)
                                    {
                                        if (tradeRecip.CanSee(this) && tradeRecip.InRange(worldLoc, GetUpdateRange(tradeRecip)))
                                        {
                                            if (ns.ContainerGridLines)
                                                ns.Send(new ContainerContentUpdate6017(this));
                                            else
                                                ns.Send(new ContainerContentUpdate(this));

                                            if (ObjectPropertyList.Enabled)
                                                ns.Send(OPLPacket);
                                        }
                                    }
                                }
                            }
                        }

                        List<Mobile> openers = contParent.Openers;

                        if (openers != null)
                        {
                            lock (openers)
                            {
                                for (int i = 0; i < openers.Count; ++i)
                                {
                                    Mobile mob = openers[i];

                                    int range = GetUpdateRange(mob);

                                    if (mob.Map != map || !mob.InRange(worldLoc, range))
                                    {
                                        openers.RemoveAt(i--);
                                    }
                                    else
                                    {
                                        if (mob == rootParent || mob == tradeRecip)
                                            continue;

                                        NetState ns = mob.NetState;

                                        if (ns != null)
                                        {
                                            if (mob.CanSee(this))
                                            {
                                                if (ns.ContainerGridLines)
                                                    ns.Send(new ContainerContentUpdate6017(this));
                                                else
                                                    ns.Send(new ContainerContentUpdate(this));

                                                if (ObjectPropertyList.Enabled)
                                                    ns.Send(OPLPacket);
                                            }
                                        }
                                    }
                                }

                                if (openers.Count == 0)
                                    contParent.Openers = null;
                            }
                        }
                        return;
                    }
                }

                if ((flags & ItemDelta.Update) != 0)
                {
                    Packet p = null;
                    Point3D worldLoc = GetWorldLocation();

                    IPooledEnumerable<NetState> eable = map.GetClientsInRange(worldLoc, GetMaxUpdateRange());

                    foreach (NetState state in eable)
                    {
                        Mobile m = state.Mobile;

                        if (m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
                        {
                            if (m_Parent == null)
                            {
                                SendInfoTo(state, ObjectPropertyList.Enabled);
                            }
                            else
                            {
                                if (p == null)
                                {
                                    if (m_Parent is Item)
                                    {
                                        if (state.ContainerGridLines)
                                            state.Send(new ContainerContentUpdate6017(this));
                                        else
                                            state.Send(new ContainerContentUpdate(this));
                                    }
                                    else if (m_Parent is Mobile)
                                    {
                                        p = new EquipUpdate(this);
                                        p.Acquire();
                                        state.Send(p);
                                    }
                                }
                                else
                                {
                                    state.Send(p);
                                }

                                if (ObjectPropertyList.Enabled)
                                {
                                    state.Send(OPLPacket);
                                }
                            }
                        }
                    }

                    if (p != null)
                        Packet.Release(p);

                    eable.Free();
                    sendOPLUpdate = false;
                }
                else if ((flags & ItemDelta.EquipOnly) != 0)
                {
                    if (m_Parent is Mobile)
                    {
                        Packet p = null;
                        Point3D worldLoc = GetWorldLocation();

                        IPooledEnumerable<NetState> eable = map.GetClientsInRange(worldLoc, GetMaxUpdateRange());

                        foreach (NetState state in eable)
                        {
                            Mobile m = state.Mobile;

                            if (m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
                            {
                                //if ( sendOPLUpdate )
                                //	state.Send( RemovePacket );

                                if (p == null)
                                    p = Packet.Acquire(new EquipUpdate(this));

                                state.Send(p);

                                if (ObjectPropertyList.Enabled)
                                    state.Send(OPLPacket);
                            }
                        }

                        Packet.Release(p);

                        eable.Free();
                        sendOPLUpdate = false;
                    }
                }

                if (sendOPLUpdate)
                {
                    Point3D worldLoc = GetWorldLocation();
                    IPooledEnumerable<NetState> eable = map.GetClientsInRange(worldLoc, GetMaxUpdateRange());

                    foreach (NetState state in eable)
                    {
                        Mobile m = state.Mobile;

                        if (m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
                            state.Send(OPLPacket);
                    }

                    eable.Free();
                }
            }
        }

        private static bool _processing = false;

        public static void ProcessDeltaQueue()
        {
#if Framework_4_0
            _processing = true;

            if (m_DeltaQueue.Count >= 512)
            {
                Parallel.ForEach(m_DeltaQueue, i => i.ProcessDelta());
            }
            else
            {
                for (int i = 0; i < m_DeltaQueue.Count; i++) m_DeltaQueue[i].ProcessDelta();
            }

            m_DeltaQueue.Clear();

            _processing = false;
#else
			int count = m_DeltaQueue.Count;

			for (int i = 0; i < m_DeltaQueue.Count; ++i) {
				m_DeltaQueue[i].ProcessDelta();

				if ( i >= count )
					break;
			}
				m_DeltaQueue.Clear();
#endif
        }

        public virtual void OnDelete()
        {
            if (this.Spawner != null)
            {
                this.Spawner.Remove(this);
                this.Spawner = null;
            }
        }

        public virtual void OnParentDeleted(object parent)
        {
            this.Delete();
        }

        public virtual void FreeCache()
        {
            ReleaseWorldPackets();
            Packet.Release(ref m_RemovePacket);
            Packet.Release(ref m_OPLPacket);
            Packet.Release(ref m_PropertyList);
        }

        public virtual void Delete()
        {
            if (Deleted)
                return;
            else if (!World.OnDelete(this))
                return;

            OnDelete();

            List<Item> items = LookupItems();

            if (items != null)
            {
                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i < items.Count)
                        items[i].OnParentDeleted(this);
                }
            }

            SendRemovePacket();

            SetFlag(ImplFlag.Deleted, true);

            if (Parent is Mobile)
                ((Mobile)Parent).RemoveItem(this);
            else if (Parent is Item)
                ((Item)Parent).RemoveItem(this);

            ClearBounce();

            if (m_Map != null)
            {
                if (m_Parent == null)
                    m_Map.OnLeave(this);
                m_Map = null;
            }

            World.RemoveItem(this);

            OnAfterDelete();

            FreeCache();
        }

        public void PublicOverheadMessage(MessageType type, int hue, bool ascii, string text)
        {
            if (m_Map != null)
            {
                Packet p = null;
                Point3D worldLoc = GetWorldLocation();

                IPooledEnumerable<NetState> eable = m_Map.GetClientsInRange(worldLoc, GetMaxUpdateRange());

                foreach (NetState state in eable)
                {
                    Mobile m = state.Mobile;

                    if (m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
                    {
                        if (p == null)
                        {
                            if (ascii)
                                p = new AsciiMessage(m_Serial, m_ItemID, type, hue, 3, this.Name, text);
                            else
                                p = new UnicodeMessage(m_Serial, m_ItemID, type, hue, 3, "ENU", this.Name, text);

                            p.Acquire();
                        }

                        state.Send(p);
                    }
                }

                Packet.Release(p);

                eable.Free();
            }
        }

        public void PublicOverheadMessage(MessageType type, int hue, int number)
        {
            PublicOverheadMessage(type, hue, number, "");
        }

        public void PublicOverheadMessage(MessageType type, int hue, int number, string args)
        {
            if (m_Map != null)
            {
                Packet p = null;
                Point3D worldLoc = GetWorldLocation();

                IPooledEnumerable<NetState> eable = m_Map.GetClientsInRange(worldLoc, GetMaxUpdateRange());

                foreach (NetState state in eable)
                {
                    Mobile m = state.Mobile;

                    if (m.CanSee(this) && m.InRange(worldLoc, GetUpdateRange(m)))
                    {
                        if (p == null)
                            p = Packet.Acquire(new MessageLocalized(m_Serial, m_ItemID, type, hue, 3, number, this.Name, args));

                        state.Send(p);
                    }
                }

                Packet.Release(p);

                eable.Free();
            }
        }

        public virtual void OnAfterDelete()
        {
        }

        public virtual void RemoveItem(Item item)
        {
            List<Item> items = LookupItems();

            if (items != null && items.Contains(item))
            {
                item.SendRemovePacket();

                items.Remove(item);

                if (!item.IsVirtualItem)
                {
                    UpdateTotal(item, TotalType.Gold, -item.TotalGold);
                    UpdateTotal(item, TotalType.Items, -(item.TotalItems + 1));
                    UpdateTotal(item, TotalType.Weight, -(item.TotalWeight + item.PileWeight));
                }

                item.Parent = null;

                item.OnRemoved(this);
                OnItemRemoved(item);
            }
        }

        public virtual void OnAfterDuped(Item newItem)
        {
        }

        public virtual bool OnDragLift(Mobile from)
        {
            return true;
        }

        public virtual bool OnEquip(Mobile from)
        {
            return true;
        }

        public ISpawner Spawner
        {
            get
            {
                CompactInfo info = LookupCompactInfo();

                if (info != null)
                    return info.m_Spawner;

                return null;

            }
            set
            {
                CompactInfo info = AcquireCompactInfo();

                info.m_Spawner = value;

                if (info.m_Spawner == null)
                    VerifyCompactInfo();
            }
        }

        public virtual void OnBeforeSpawn(Point3D location, Map m)
        {
        }

        public virtual void OnAfterSpawn()
        {
        }

        public virtual int PhysicalResistance { get { return 0; } }
        public virtual int FireResistance { get { return 0; } }
        public virtual int ColdResistance { get { return 0; } }
        public virtual int PoisonResistance { get { return 0; } }
        public virtual int EnergyResistance { get { return 0; } }

        [CommandProperty(AccessLevel.Counselor)]
        public Serial Serial
        {
            get
            {
                return m_Serial;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public IEntity ParentEntity
        {
            get
            {
                IEntity p = Parent as IEntity;

                return p;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public IEntity RootParentEntity
        {
            get
            {
                IEntity p = RootParent as IEntity;

                return p;
            }
        }

        #region Location Location Location!

        public virtual void OnLocationChange(Point3D oldLocation)
        {
        }

        //public virtual void OnInternalLocationChange(Point3D oldLocation)
        //{
        //}

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public virtual Point3D Location
        {
            get
            {
                return m_Location;
            }
            set
            {
                Point3D oldLocation = m_Location;

                if (oldLocation != value)
                {
                    if (m_Map != null)
                    {
                        if (m_Parent == null)
                        {
                            IPooledEnumerable<NetState> eable;

                            if (m_Location.m_X != 0)
                            {
                                eable = m_Map.GetClientsInRange(oldLocation, GetMaxUpdateRange());

                                foreach (NetState state in eable)
                                {
                                    Mobile m = state.Mobile;

                                    if (!m.InRange(value, GetUpdateRange(m)))
                                    {
                                        state.Send(this.RemovePacket);
                                    }
                                }

                                eable.Free();
                            }

                            Point3D oldLoc = m_Location;
                            m_Location = value;
                            ReleaseWorldPackets();

                            SetLastMoved();

                            eable = m_Map.GetClientsInRange(m_Location, GetMaxUpdateRange());

                            foreach (NetState state in eable)
                            {
                                Mobile m = state.Mobile;

                                if (m.CanSee(this) && m.InRange(m_Location, GetUpdateRange(m)) && (!state.HighSeas || !m_NoMoveHS || (m_DeltaFlags & ItemDelta.Update) != 0 || !m.InRange(oldLoc, GetUpdateRange(m))))
                                    SendInfoTo(state);
                            }

                            eable.Free();

                            RemDelta(ItemDelta.Update);
                        }
                        else if (m_Parent is Item)
                        {
                            m_Location = value;
                            ReleaseWorldPackets();

                            Delta(ItemDelta.Update);
                        }
                        else
                        {
                            m_Location = value;
                            ReleaseWorldPackets();
                        }

                        if (m_Parent == null)
                            m_Map.OnMove(oldLocation, this);
                    }
                    else
                    {
                        m_Location = value;
                        ReleaseWorldPackets();
                    }

                    this.OnLocationChange(oldLocation);
                }
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int X
        {
            get { return m_Location.m_X; }
            set { Location = new Point3D(value, m_Location.m_Y, m_Location.m_Z); }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int Y
        {
            get { return m_Location.m_Y; }
            set { Location = new Point3D(m_Location.m_X, value, m_Location.m_Z); }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int Z
        {
            get { return m_Location.m_Z; }
            set { Location = new Point3D(m_Location.m_X, m_Location.m_Y, value); }
        }
        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int ItemID
        {
            get
            {
                return m_ItemID;
            }
            set
            {
                if (m_ItemID != value)
                {
                    int oldPileWeight = this.PileWeight;

                    m_ItemID = value;
                    ReleaseWorldPackets();

                    int newPileWeight = this.PileWeight;

                    UpdateTotal(this, TotalType.Weight, newPileWeight - oldPileWeight);

                    InvalidateProperties();
                    Delta(ItemDelta.Update);
                }
            }
        }

        public virtual string DefaultName
        {
            get { return null; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Name
        {
            get
            {
                CompactInfo info = LookupCompactInfo();

                if (info != null && info.m_Name != null)
                    return info.m_Name;

                return this.DefaultName;
            }
            set
            {
                if (value == null || value != DefaultName)
                {
                    CompactInfo info = AcquireCompactInfo();

                    info.m_Name = value;

                    if (info.m_Name == null)
                        VerifyCompactInfo();

                    InvalidateProperties();
                }
            }
        }

        public virtual object Parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                if (m_Parent == value)
                    return;

                object oldParent = m_Parent;

                m_Parent = value;

                if (m_Map != null)
                {
                    if (oldParent != null && m_Parent == null)
                        m_Map.OnEnter(this);
                    else if (m_Parent != null)
                        m_Map.OnLeave(this);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public LightType Light
        {
            get
            {
                return (LightType)m_Direction;
            }
            set
            {
                if ((LightType)m_Direction != value)
                {
                    m_Direction = (Direction)value;
                    ReleaseWorldPackets();

                    Delta(ItemDelta.Update);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                if (m_Direction != value)
                {
                    m_Direction = value;
                    ReleaseWorldPackets();

                    Delta(ItemDelta.Update);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Amount
        {
            get
            {
                return m_Amount;
            }
            set
            {
                int oldValue = m_Amount;

                if (oldValue != value)
                {
                    int oldPileWeight = this.PileWeight;

                    m_Amount = value;
                    ReleaseWorldPackets();

                    int newPileWeight = this.PileWeight;

                    UpdateTotal(this, TotalType.Weight, newPileWeight - oldPileWeight);

                    OnAmountChange(oldValue);

                    Delta(ItemDelta.Update);

                    if (oldValue > 1 || value > 1)
                        InvalidateProperties();

                    if (!Stackable && m_Amount > 1)
                        Console.WriteLine("Warning: 0x{0:X}: Amount changed for non-stackable item '{2}'. ({1})", m_Serial.Value, m_Amount, GetType().Name);
                }
            }
        }

        protected virtual void OnAmountChange(int oldValue)
        {
        }

        public virtual bool HandlesOnSpeech { get { return false; } }

        public virtual void OnSpeech(SpeechEventArgs e)
        {
        }

        public virtual bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (!from.AllowTrades || (Nontransferable && from.Player && from.AccessLevel < AccessLevel.GameMaster))
            {
                HandleInvalidTransfer(from);
                return false;
            }

            return true;
        }

        public virtual bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null)
                return false;

            else if (from.AccessLevel < AccessLevel.GameMaster && !from.InRange(target.Location, 2))
                return false;

            else if (!from.CanSee(target) || !from.InLOS(target))
                return false;

            else if (!from.OnDroppedItemToMobile(this, target))
                return false;

            else if (!OnDroppedToMobile(from, target))
                return false;

            else if (!target.OnDragDrop(from, this))
                return false;

            else
                return true;
        }

        public virtual bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (!from.OnDroppedItemInto(this, target, p))
            {
                return false;
            }
            else if (Nontransferable && from.Player && target != from.Backpack && from.AccessLevel < AccessLevel.GameMaster)
            {
                HandleInvalidTransfer(from);
                return false;
            }

            return target.OnDragDropInto(from, this, p);
        }

        public virtual bool OnDroppedOnto(Mobile from, Item target)
        {
            return OnDroppedOnto(from, target, target.Location);
        }

        //Overloaded function with Point3D added by Sean (IPY)
        public virtual bool OnDroppedOnto(Mobile from, Item target, Point3D p)
        {
            if (Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null)
                return false;
            else if (from.AccessLevel < AccessLevel.GameMaster && !from.InRange(target.GetWorldLocation(), 2))
                return false;
            else if (!from.CanSee(target) || !from.InLOS(target))
                return false;
            else if (!target.IsAccessibleTo(from))
                return false;
            else if (!from.OnDroppedItemOnto(this, target))
                return false;
            else if (Nontransferable && from.Player && target != from.Backpack && from.AccessLevel < AccessLevel.GameMaster)
            {
                HandleInvalidTransfer(from);
                return false;
            }
            else
                return target.OnDragDrop(from, this);
        }

        public virtual bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (Deleted || from.Deleted || target.Deleted || from.Map != target.Map || from.Map == null || target.Map == null)
                return false;

            object root = target.RootParent;

            if (from.AccessLevel < AccessLevel.GameMaster && !from.InRange(target.GetWorldLocation(), 2))
                return false;
            else if (!from.CanSee(target) || !from.InLOS(target))
                return false;
            else if (!target.IsAccessibleTo(from))
                return false;
            else if (root is Mobile && !((Mobile)root).CheckNonlocalDrop(from, this, target))
                return false;
            else if (!from.OnDroppedItemToItem(this, target, p))
                return false;
            else if (target is Container && p.m_X != -1 && p.m_Y != -1)
                return OnDroppedInto(from, (Container)target, p);
            else
                return OnDroppedOnto(from, target, p);
        }

        public virtual bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (Nontransferable && from.Player && from.AccessLevel < AccessLevel.GameMaster)
            {
                HandleInvalidTransfer(from);
                return false;
            }

            return true;
        }

        public virtual int GetLiftSound(Mobile from)
        {
            return 0x57;
        }

        private static int m_OpenSlots;

        public virtual bool DropToWorld(Mobile from, Point3D p)
        {
            if (Deleted || from.Deleted || from.Map == null)
                return false;

            else if (!from.InRange(p, 2))
                return false;

            Map map = from.Map;

            if (map == null)
                return false;

            int x = p.m_X, y = p.m_Y;
            int z = int.MinValue;

            int maxZ = from.Z + 16;

            LandTile landTile = map.Tiles.GetLandTile(x, y);
            TileFlag landFlags = TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags;

            int landZ = 0, landAvg = 0, landTop = 0;
            map.GetAverageZ(x, y, ref landZ, ref landAvg, ref landTop);

            if (!landTile.Ignored && (landFlags & TileFlag.Impassable) == 0)
            {
                if (landAvg <= maxZ)
                    z = landAvg;
            }

            StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, true);

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile tile = tiles[i];
                ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                if (!id.Surface)
                    continue;

                int top = tile.Z + id.CalcHeight;

                if (top > maxZ || top < z)
                    continue;

                z = top;
            }

            List<Item> items = new List<Item>();

            IPooledEnumerable<Item> eable = map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
                    continue;

                items.Add(item);

                ItemData id = item.ItemData;

                if (!id.Surface)
                    continue;

                int top = item.Z + id.CalcHeight;

                if (top > maxZ || top < z)
                    continue;

                z = top;
            }

            eable.Free();

            if (z == int.MinValue)
                return false;

            if (z > maxZ)
                return false;

            m_OpenSlots = (1 << 20) - 1;

            int surfaceZ = z;

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile tile = tiles[i];
                ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                int checkZ = tile.Z;
                int checkTop = checkZ + id.CalcHeight;

                if (checkTop == checkZ && !id.Surface)
                    ++checkTop;

                int zStart = checkZ - z;
                int zEnd = checkTop - z;

                if (zStart >= 20 || zEnd < 0)
                    continue;

                if (zStart < 0)
                    zStart = 0;

                if (zEnd > 19)
                    zEnd = 19;

                int bitCount = zEnd - zStart;

                m_OpenSlots &= ~(((1 << bitCount) - 1) << zStart);
            }

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];
                ItemData id = item.ItemData;

                int checkZ = item.Z;
                int checkTop = checkZ + id.CalcHeight;

                if (checkTop == checkZ && !id.Surface)
                    ++checkTop;

                int zStart = checkZ - z;
                int zEnd = checkTop - z;

                if (zStart >= 20 || zEnd < 0)
                    continue;

                if (zStart < 0)
                    zStart = 0;

                if (zEnd > 19)
                    zEnd = 19;

                int bitCount = zEnd - zStart;

                m_OpenSlots &= ~(((1 << bitCount) - 1) << zStart);
            }

            int height = ItemData.Height;

            if (height == 0)
                ++height;

            if (height > 30)
                height = 30;

            int match = (1 << height) - 1;
            bool okay = false;

            for (int i = 0; i < 20; ++i)
            {
                if ((i + height) > 20)
                    match >>= 1;

                okay = ((m_OpenSlots >> i) & match) == match;

                if (okay)
                {
                    z += i;
                    break;
                }
            }

            if (!okay)
                return false;

            height = ItemData.Height;

            if (height == 0)
                ++height;

            if (landAvg > z && (z + height) > landZ)
                return false;
            else if ((landFlags & TileFlag.Impassable) != 0 && landAvg > surfaceZ && (z + height) > landZ)
                return false;

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile tile = tiles[i];
                ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                int checkZ = tile.Z;
                int checkTop = checkZ + id.CalcHeight;

                if (checkTop > z && (z + height) > checkZ)
                    return false;
                else if ((id.Surface || id.Impassable) && checkTop > surfaceZ && (z + height) > checkZ)
                    return false;
            }

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];
                ItemData id = item.ItemData;

                //int checkZ = item.Z;
                //int checkTop = checkZ + id.CalcHeight;

                if ((item.Z + id.CalcHeight) > z && (z + height) > item.Z)
                    return false;
            }

            p = new Point3D(x, y, z);

            if (!from.InLOS(new Point3D(x, y, z + 1)))
                return false;
            else if (!from.OnDroppedItemToWorld(this, p))
                return false;
            else if (!OnDroppedToWorld(from, p))
                return false;

            int soundID = GetDropSound();

            MoveToWorld(p, from.Map);

            from.SendSound(soundID == -1 ? 0x42 : soundID, GetWorldLocation());

            return true;
        }

        public void SendRemovePacket()
        {
            if (!Deleted && m_Map != null)
            {
                Point3D worldLoc = GetWorldLocation();

                IPooledEnumerable<NetState> eable = m_Map.GetClientsInRange(worldLoc, GetMaxUpdateRange());

                foreach (NetState state in eable)
                {
                    Mobile m = state.Mobile;

                    if (m.InRange(worldLoc, GetUpdateRange(m)))
                    {
                        state.Send(this.RemovePacket);
                    }
                }

                eable.Free();
            }
        }

        public virtual int GetDropSound()
        {
            return -1;
        }

        public Point3D GetWorldLocation()
        {
            object root = RootParent;

            if (root == null)
                return m_Location;
            else
                return ((IEntity)root).Location;

            //return root == null ? m_Location : new Point3D( (IPoint3D) root );
        }

        public virtual bool BlocksFit { get { return false; } }

        public Point3D GetSurfaceTop()
        {
            object root = RootParent;

            if (root == null)
                return new Point3D(m_Location.m_X, m_Location.m_Y, m_Location.m_Z + (ItemData.Surface ? ItemData.CalcHeight : 0));
            else
                return ((IEntity)root).Location;
        }

        public Point3D GetWorldTop()
        {
            object root = RootParent;

            if (root == null)
                return new Point3D(m_Location.m_X, m_Location.m_Y, m_Location.m_Z + ItemData.CalcHeight);
            else
                return ((IEntity)root).Location;
        }

        public void SendLocalizedMessageTo(Mobile to, int number)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new MessageLocalized(Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", ""));
        }

        public void SendLocalizedMessageTo(Mobile to, int number, string args)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new MessageLocalized(Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", args));
        }

        public void SendLocalizedMessageTo(Mobile to, int number, AffixType affixType, string affix, string args)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new MessageLocalizedAffix(Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", affixType, affix, args));
        }

        #region OnDoubleClick[...]

        public virtual void OnDoubleClick(Mobile from)
        {
        }

        public virtual void OnDoubleClickOutOfRange(Mobile from)
        {
        }

        public virtual void OnDoubleClickCantSee(Mobile from)
        {
        }

        public virtual void OnDoubleClickDead(Mobile from)
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019048); // I am dead and cannot do that.
        }

        public virtual void OnDoubleClickNotAccessible(Mobile from)
        {
            from.SendLocalizedMessage(500447); // That is not accessible.
        }

        public virtual void OnDoubleClickSecureTrade(Mobile from)
        {
            from.SendLocalizedMessage(500447); // That is not accessible.
        }
        #endregion

        public virtual void OnSnoop(Mobile from)
        {
        }

        public bool InSecureTrade
        {
            get
            {
                return (GetSecureTradeCont() != null);
            }
        }

        public SecureTradeContainer GetSecureTradeCont()
        {
            object p = this;

            while (p is Item)
            {
                if (p is SecureTradeContainer)
                    return (SecureTradeContainer)p;

                p = ((Item)p).m_Parent;
            }

            return null;
        }

        public virtual void OnItemAdded(Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnSubItemAdded(item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnSubItemAdded(item);
        }

        public virtual void OnItemRemoved(Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnSubItemRemoved(item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnSubItemRemoved(item);
        }

        public virtual void OnSubItemAdded(Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnSubItemAdded(item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnSubItemAdded(item);
        }

        public virtual void OnSubItemRemoved(Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnSubItemRemoved(item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnSubItemRemoved(item);
        }

        public virtual void OnItemBounceCleared(Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnSubItemBounceCleared(item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnSubItemBounceCleared(item);
        }

        public virtual void OnSubItemBounceCleared(Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnSubItemBounceCleared(item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnSubItemBounceCleared(item);
        }

        public virtual bool CheckTarget(Mobile from, Server.Targeting.Target targ, object targeted)
        {
            if (m_Parent is Item)
                return ((Item)m_Parent).CheckTarget(from, targ, targeted);
            else if (m_Parent is Mobile)
                return ((Mobile)m_Parent).CheckTarget(from, targ, targeted);

            return true;
        }

        public virtual bool IsAccessibleTo(Mobile check)
        {
            if (AlwaysAllowDoubleClick)
                return true;

            if (m_Parent is Item)
                return ((Item)m_Parent).IsAccessibleTo(check);

            Region reg = Region.Find(GetWorldLocation(), m_Map);

            return reg.CheckAccessibility(this, check);

            /*SecureTradeContainer cont = GetSecureTradeCont();

            if ( cont != null && !cont.IsChildOf( check ) )
                return false;

            return true;*/
        }

        public bool IsChildOf(object o)
        {
            return IsChildOf(o, false);
        }

        public bool IsChildOf(object o, bool allowNull)
        {
            object p = m_Parent;

            if ((p == null || o == null) && !allowNull)
                return false;

            if (p == o)
                return true;

            while (p is Item)
            {
                Item item = (Item)p;

                if (item.m_Parent == null)
                {
                    break;
                }
                else
                {
                    p = item.m_Parent;

                    if (p == o)
                        return true;
                }
            }

            return false;
        }

        public ItemData ItemData
        {
            get
            {
                return TileData.ItemTable[m_ItemID & TileData.MaxItemValue];
            }
        }

        public virtual void OnItemUsed(Mobile from, Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnItemUsed(from, item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnItemUsed(from, item);
        }

        public bool CheckItemUse(Mobile from)
        {
            return CheckItemUse(from, this);
        }

        public virtual bool CheckItemUse(Mobile from, Item item)
        {
            if (m_Parent is Item)
                return ((Item)m_Parent).CheckItemUse(from, item);
            else if (m_Parent is Mobile)
                return ((Mobile)m_Parent).CheckItemUse(from, item);
            else
                return true;
        }

        public virtual void OnItemLifted(Mobile from, Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnItemLifted(from, item);
            else if (m_Parent is Mobile)
                ((Mobile)m_Parent).OnItemLifted(from, item);
        }

        public bool CheckLift(Mobile from)
        {
            LRReason reject = LRReason.Inspecific;

            return CheckLift(from, this, ref reject);
        }

        public virtual bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (m_Parent is Item)
                return ((Item)m_Parent).CheckLift(from, item, ref reject);

            else if (m_Parent is Mobile)
                return ((Mobile)m_Parent).CheckLift(from, item, ref reject);

            else
                return true;
        }

        public virtual bool CanTarget { get { return true; } }
        public virtual bool DisplayLootType { get { return true; } }

        public virtual void OnSingleClickContained(Mobile from, Item item)
        {
            if (m_Parent is Item)
                ((Item)m_Parent).OnSingleClickContained(from, item);
        }

        public virtual void OnAosSingleClick(Mobile from)
        {
            ObjectPropertyList opl = this.PropertyList;

            if (opl.Header > 0)
                from.Send(new MessageLocalized(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, opl.Header, this.Name, opl.HeaderArgs));
        }

        public virtual void DisplayLabelName(Mobile from)
        {
            if (from == null)
                return;

            if (Name == null)
            {
                NetState netstate = from.NetState;

                if (netstate != null)
                {
                    if (m_Amount <= 1)
                        netstate.Send(new MessageLocalized(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, LabelNumber, "", ""));

                    else
                        netstate.Send(new MessageLocalizedAffix(m_Serial, m_ItemID, MessageType.Label, 0x3B2, 3, LabelNumber, "", AffixType.Append, String.Format(" : {0}", m_Amount), ""));
                }
            }

            else
            {
                if (Amount > 1)
                    LabelTo(from, Name + " : " + Amount);

                else
                    LabelTo(from, Name);
            }
        }

        public virtual void OnSingleClick(Mobile from)
        {
            if (Deleted || !from.CanSee(this))
                return;

            DisplayLabelName(from);

            if (DecorativeEquipment)
            {
                switch (LootType)
                {
                    case LootType.Regular: LabelTo(from, "(decorative)"); break;
                    case LootType.Blessed: LabelTo(from, "(blessed decorative)"); break;
                    case LootType.Newbied: LabelTo(from, "(newbied decorative)"); break;
                    case LootType.Cursed: LabelTo(from, "(cursed decorative)"); break;
                }
            }

            else
            {
                switch (LootType)
                {
                    case LootType.Blessed: LabelTo(from, "(blessed)"); break;
                    case LootType.Newbied: LabelTo(from, "(newbied)"); break;
                    case LootType.Cursed: LabelTo(from, "(cursed)"); break;
                }
            }

            if (DisplayCrafter && CrafterName != "")
                LabelTo(from, "[crafted by " + CrafterName + "]");
        }

        private static bool m_ScissorCopyLootType;

        public static bool ScissorCopyLootType
        {
            get { return m_ScissorCopyLootType; }
            set { m_ScissorCopyLootType = value; }
        }

        public virtual void ScissorHelper(Mobile from, Item newItem, int amountPerOldItem)
        {
            ScissorHelper(from, newItem, amountPerOldItem, true);
        }

        public virtual void ScissorHelper(Mobile from, Item newItem, int amountPerOldItem, bool carryHue)
        {
            int amount = Amount;

            if (amount > (60000 / amountPerOldItem)) // let's not go over 60000
                amount = (60000 / amountPerOldItem);

            Amount -= amount;

            int ourHue = Hue;
            Map thisMap = this.Map;
            object thisParent = this.m_Parent;
            Point3D worldLoc = this.GetWorldLocation();
            LootType type = this.LootType;

            if (Amount == 0)
                Delete();

            newItem.Amount = amount * amountPerOldItem;

            if (carryHue)
                newItem.Hue = ourHue;

            if (m_ScissorCopyLootType)
                newItem.LootType = type;

            if (!(thisParent is Container) || !((Container)thisParent).TryDropItem(from, newItem, false))
                newItem.MoveToWorld(worldLoc, thisMap);
        }

        public virtual void Consume()
        {
            Consume(1);
        }

        public virtual void Consume(int amount)
        {
            this.Amount -= amount;

            if (this.Amount <= 0)
                this.Delete();
        }

        public virtual void ReplaceWith(Item newItem)
        {
            if (m_Parent is Container)
            {
                ((Container)m_Parent).AddItem(newItem);
                newItem.Location = m_Location;
            }
            else
            {
                newItem.MoveToWorld(GetWorldLocation(), m_Map);
            }

            Delete();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool QuestItem
        {
            get { return GetFlag(ImplFlag.QuestItem); }
            set
            {
                SetFlag(ImplFlag.QuestItem, value);

                InvalidateProperties();

                ReleaseWorldPackets();

                Delta(ItemDelta.Update);
            }
        }

        public bool Insured
        {
            get { return GetFlag(ImplFlag.Insured); }
            set { SetFlag(ImplFlag.Insured, value); InvalidateProperties(); }
        }

        public bool PayedInsurance
        {
            get { return GetFlag(ImplFlag.PayedInsurance); }
            set { SetFlag(ImplFlag.PayedInsurance, value); }
        }

        public Mobile BlessedFor
        {
            get
            {
                CompactInfo info = LookupCompactInfo();

                if (info != null)
                    return info.m_BlessedFor;

                return null;
            }
            set
            {
                CompactInfo info = AcquireCompactInfo();

                info.m_BlessedFor = value;

                if (info.m_BlessedFor == null)
                    VerifyCompactInfo();

                InvalidateProperties();
            }
        }

        public virtual bool CheckBlessed(object obj, bool isOndDeath = false)
        {
            return CheckBlessed(obj as Mobile);
        }

        public virtual bool CheckBlessed(Mobile m, bool isOnDeath = false)
        {
            if (m_LootType == LootType.Blessed || (Mobile.InsuranceEnabled && Insured))
                return true;

            return (m != null && m == this.BlessedFor);
        }

        public virtual bool CheckNewbied()
        {
            return (m_LootType == LootType.Newbied);
        }

        public virtual bool IsStandardLoot()
        {
            if (Mobile.InsuranceEnabled && Insured)
                return false;

            if (this.BlessedFor != null)
                return false;

            return (m_LootType == LootType.Regular);
        }

        public override string ToString()
        {
            return String.Format("0x{0:X} \"{1}\"", m_Serial.Value, GetType().Name);
        }

        internal int m_TypeRef;

        public Item()
        {
            m_Serial = Serial.NewItem;

            //m_Items = new ArrayList( 1 );
            Visible = true;
            Movable = true;
            Amount = 1;
            m_Map = Map.Internal;

            SetLastMoved();

            World.AddItem(this);

            Type ourType = this.GetType();
            m_TypeRef = World.m_ItemTypes.IndexOf(ourType);

            if (m_TypeRef == -1)
            {
                World.m_ItemTypes.Add(ourType);
                m_TypeRef = World.m_ItemTypes.Count - 1;
            }
        }

        [Constructable]
        public Item(int itemID): this()
        {
            m_ItemID = itemID;

            Resource = DefaultResource; 
        }

        public Item(Serial serial)
        {
            m_Serial = serial;

            Type ourType = this.GetType();
            m_TypeRef = World.m_ItemTypes.IndexOf(ourType);

            if (m_TypeRef == -1)
            {
                World.m_ItemTypes.Add(ourType);
                m_TypeRef = World.m_ItemTypes.Count - 1;
            }
        }

        public virtual void OnSectorActivate()
        {
        }

        public virtual void OnSectorDeactivate()
        {
        }
    }
}