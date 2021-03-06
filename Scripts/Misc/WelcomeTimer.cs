using System;
using Server.Network;
using Server.Items;
using Server.Gumps;

namespace Server.Misc
{
	public class WelcomeTimer : Timer
	{
		private Mobile m_Mobile;
		private int m_State, m_Count;

		private static string[] m_Messages = new string[]
		{
			"",
		};

		public WelcomeTimer( Mobile m ) : this( m, m_Messages.Length )
		{
		}

		public WelcomeTimer( Mobile m, int count ) : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 10.0 ) )
		{
			m_Mobile = m;
			m_Count = count;
		}

		protected override void OnTick()
		{
            if (m_State < m_Count)
            {
                m_Mobile.SendMessage(0x35, m_Messages[m_State++]);
                //m_Mobile.SendGump(new Server.Gumps.StartLocationGump(m_Mobile, Map.Felucca, 2));
            }

            if (m_State == m_Count)
                Stop();
		}
	}
}