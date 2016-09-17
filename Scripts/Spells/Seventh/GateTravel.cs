using System;
using Server.Network;
using Server.Multis;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Regions;
using Server.Mobiles;


using Server.Multis;
using Server.Custom;

namespace Server.Spells.Seventh
{
	public class GateTravelSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Gate Travel", "Vas Rel Por",
				263,
				9032,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public override TimeSpan GetCastDelay()
		{
		    return TimeSpan.FromSeconds( 4.0 );
		}

		private RunebookEntry m_Entry;
        private Runebook m_Book;

        private RuneTomeRuneEntry m_RunebookRuneEntry;
        private RuneTome m_RuneTome;

        public GateTravelSpell(Mobile caster, Item scroll): this(caster, scroll, null, null, null, null)
		{
		}

        public GateTravelSpell(Mobile caster, Item scroll, RunebookEntry entry, Runebook book, RuneTomeRuneEntry recallRuneEntry, RuneTome runeTome): base(caster, scroll, m_Info)
		{
            m_Entry = entry;
            m_Book = book;

            m_RunebookRuneEntry = recallRuneEntry;
            m_RuneTome = runeTome;
		}

        public override void GetCastSkills(out double min, out double max)
        {
            if (m_Book != null || m_RuneTome != null)
            {
                min = 50;
                max = 50;
            }

            else
                base.GetCastSkills(out min, out max);
        }

		public override void OnCast()
		{
            if (m_Entry != null)
                Effect(m_Entry.Location, m_Entry.Map, true);

            else if (m_RunebookRuneEntry != null)
                Effect(m_RunebookRuneEntry.m_Target, m_RunebookRuneEntry.m_TargetMap, true);

            else
                Caster.Target = new InternalTarget(this);		

            /*
			if ( m_Entry == null )
				Caster.Target = new InternalTarget( this );

			else
				Effect( m_Entry.Location, m_Entry.Map, true );
             * */
		}
		
        public override bool CheckCast()
		{
            PlayerMobile pm_Caster = Caster as PlayerMobile;

            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.GateOut, Caster.Location, Caster.Map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventGateInResponse != "")
                    Caster.SendMessage(recallBlocker.PreventGateInResponse);

                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultGateInResponse);

                return false;
            }
            
            else if (Caster.Criminal)
            {
                Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }            

            else if (SpellHelper.CheckCombat(Caster))
            {
                Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }

            else if (BaseShip.FindShipAt(Caster.Location, Caster.Map) != null)
            {
                Caster.SendMessage("You may not cast this spell while at sea.");
                return false;
            }

            if (pm_Caster != null)
            {
                if (pm_Caster.RecallRestrictionExpiration > DateTime.UtcNow)
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, pm_Caster.RecallRestrictionExpiration, false, true, true, true, true);

                    pm_Caster.SendMessage("You are unable to cast this spell for another " + timeRemaining + ".");

                    return false;
                }
            }

            return SpellHelper.CheckTravel(Caster, TravelCheckType.GateFrom);
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.GateIn, loc, map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventGateOutResponse != "")
                    Caster.SendMessage(recallBlocker.PreventGateOutResponse);

                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultGateOutResponse);               
            }

			else if ( map == null || (!Core.AOS && Caster.Map != map) )			
				Caster.SendLocalizedMessage( 1005570 ); // You can not gate to another facet.			

            else if (!SpellHelper.CheckTravel(Caster, map, loc, TravelCheckType.GateTo))            
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...

			else if ( map != Map.Felucca )			
				Caster.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.			

			else if ( Caster.ShortTermMurders >= 5 && map != Map.Felucca )			
				Caster.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.			

			else if (!SpellHelper.CheckIfOK(Caster.Map, loc.X, loc.Y, loc.Z))
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.			

			else if ( (checkMulti && SpellHelper.CheckMulti( loc, map )) )			
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.			

			else if ( SpellHelper.IsSolenHiveLoc( loc ) )			
				Caster.SendLocalizedMessage( 501802 ); // Thy spell doth not appear to work...			

			else if ( SpellHelper.IsStarRoom( loc ) )			
				Caster.SendLocalizedMessage( 501802 ); // Thy spell doth not appear to work...			

			else if ( SpellHelper.IsWindLoc( Caster.Location ) )			
				Caster.SendLocalizedMessage( 501802 ); // Thy spell doth not appear to work...			

			else if ( SpellHelper.IsWindLoc( loc ) )			
				Caster.SendLocalizedMessage( 501802 ); // Thy spell doth not appear to work...			

            else if (BaseShip.FindShipAt(loc, map) != null)            
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...     

            else if (m_RuneTome != null && m_RuneTome.GateCharges <= 0)
                Caster.SendMessage("There are no gate charges left on that item.");

			else if ( CheckSequence() && CheckCast() )
			{
                if (m_RuneTome != null)
                    --m_RuneTome.GateCharges;

				Caster.SendLocalizedMessage( 501024 ); // You open a magical gate to another location

                //Player Enhancement Customization: Traveler
                bool traveler = false; //PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.Traveler);

                if (traveler)
                {
                    //First Gate
                    Effects.PlaySound(Caster.Location, Caster.Map, 0x5CE);
                    Effects.SendLocationParticles(EffectItem.Create(Caster.Location, Caster.Map, TimeSpan.FromSeconds(0.5)), 6899, 10, 30, 0, 0, 5029, 0);

                    InternalItem firstGate = new InternalItem(loc, map);
                    firstGate.Visible = false;
                    firstGate.MoveToWorld(Caster.Location, Caster.Map);

                    //Second Gate
                    Effects.PlaySound(loc, map, 0x5CE);
                    Effects.SendLocationParticles(EffectItem.Create(loc, map, TimeSpan.FromSeconds(0.5)), 6899, 10, 30, 0, 0, 5029, 0);

                    InternalItem secondGate = new InternalItem(Caster.Location, Caster.Map);
                    secondGate.Visible = false;
                    secondGate.MoveToWorld(loc, map);

                    Timer.DelayCall(TimeSpan.FromSeconds(1.25), delegate
                    {
                        if (firstGate != null)
                        {
                            if (!firstGate.Deleted)
                                firstGate.Visible = true;
                        }

                        if (secondGate != null)
                        {
                            if (!secondGate.Deleted)
                                secondGate.Visible = true;
                        }
                    });
                }

                else
                {
                    Effects.PlaySound(Caster.Location, Caster.Map, 0x20E);

                    InternalItem firstGate = new InternalItem(loc, map);
                    firstGate.MoveToWorld(Caster.Location, Caster.Map);

                    Effects.PlaySound(loc, map, 0x20E);

                    InternalItem secondGate = new InternalItem(Caster.Location, Caster.Map);
                    secondGate.MoveToWorld(loc, map);
                }
			}

			FinishSequence();
		}
        
		[DispellableField]
		private class InternalItem : Moongate
		{
			public override bool ShowFeluccaWarning{ get{ return Core.AOS; } }

			public InternalItem( Point3D target, Map map ) : base( target, map )
			{
				Map = map;
               
				if ( ShowFeluccaWarning && map == Map.Felucca )
					ItemID = 0xDDA;

				Dispellable = true;

				InternalTimer t = new InternalTimer( this );
				t.Start();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				Delete();
			}

			public override void OnGateUsed(Mobile m)
			{
				base.OnGateUsed(m);
			}

			private class InternalTimer : Timer
			{
				private Item m_Item;

				public InternalTimer( Item item ) : base( TimeSpan.FromSeconds( 30.0 ) )
				{                    
					m_Item = item;
				}

				protected override void OnTick()
				{
					m_Item.Delete();
				}
			}
		}

		private class InternalTarget : Target
		{
			private GateTravelSpell m_Owner;

			public InternalTarget( GateTravelSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;

				owner.Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 501029 ); // Select Marked item.
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune)o;

					if ( rune.Marked )
						m_Owner.Effect( rune.Target, rune.TargetMap, true );

					else
						from.SendLocalizedMessage( 501803 ); // That rune is not yet marked.
				}

				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook)o).Default;

					if ( e != null )
						m_Owner.Effect( e.Location, e.Map, true );

					else
						from.SendLocalizedMessage( 502354 ); // Target is not marked.
				}

				else if ( o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseShip )
				{
				//	BaseShip ship = ((Key)o).Link as BaseShip;

				//	if ( !ship.Deleted && ship.CheckKey( ((Key)o).KeyValue ) )
				//		m_Owner.Effect( ship.GetMarkedLocation(), ship.Map, false );
				//	else

					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501030, from.Name, "" ) ); // I can not gate travel from that object.
				}

                else if (o is RuneTome)
                {
                    RuneTome runeTome = o as RuneTome;

                    RuneTomeRuneEntry defaultRuneEntry = null;

                    foreach (RuneTomeRuneEntry entry in runeTome.m_RecallRuneEntries)
                    {
                        if (entry == null)
                            continue;

                        if (entry.m_IsDefaultRune)
                        {
                            defaultRuneEntry = entry;
                            break;
                        }
                    }

                    if (defaultRuneEntry == null)
                    {
                        if (runeTome.m_RecallRuneEntries.Count > 0)
                            defaultRuneEntry = runeTome.m_RecallRuneEntries[0];

                        else
                        {
                            from.SendMessage("There are no recall runes stored within this rune tome.");
                            return;
                        }
                    }

                    if (defaultRuneEntry != null)
                        m_Owner.Effect(defaultRuneEntry.m_Target, defaultRuneEntry.m_TargetMap, true);
                }

				else if ( o is HouseRaffleDeed && ((HouseRaffleDeed)o).ValidLocation() )
				{
					HouseRaffleDeed deed = (HouseRaffleDeed)o;

					m_Owner.Effect( deed.PlotLocation, deed.PlotFacet, true );
				}

				else				
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501030, from.Name, "" ) ); // I can not gate travel from that object.
			}
			
			protected override void OnNonlocalTarget( Mobile from, object o )
			{
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}