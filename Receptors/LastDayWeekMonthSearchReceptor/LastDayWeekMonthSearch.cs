/*
    Copyright 2104 Higher Order Programming

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using Clifton.ExtensionMethods;
using Clifton.MycroParser;
using Clifton.Tools.Strings.Extensions;

using Clifton.Receptor.Interfaces;
using Clifton.SemanticTypeSystem.Interfaces;

namespace LastDayWeekMonthSearchReceptor
{
	/// <summary>
	/// Displays an input form for the user to enter a search string.
	/// </summary>
	public class LastDayWeekMonthSearch : BaseReceptor
	{
		public override string Name { get { return "Date Range Search"; } }

		protected Form form;

		public LastDayWeekMonthSearch(IReceptorSystem rsys)
			: base(rsys)
		{
			AddEmitProtocol("SearchDateRange");
			CreateForm();
		}

		public override void Terminate()
		{
			form.Close();
		}

		protected void CreateForm()
		{
			MycroParser mp = new MycroParser();
			XmlDocument doc = new XmlDocument();
			doc.Load("SearchForWithinPastDate.xml");
			mp.Load(doc, "Form", this);
			form = (Form)mp.Process();
			form.Show();

			RadioButton rbToday = (RadioButton)mp.ObjectCollection["rbToday"];
			RadioButton rbLastWeek = (RadioButton)mp.ObjectCollection["rbLastSeven"];
			RadioButton rbLastMonth = (RadioButton)mp.ObjectCollection["rbLastMonth"];
			RadioButton rbLastYear = (RadioButton)mp.ObjectCollection["rbLastYear"];
			RadioButton rbAll = (RadioButton)mp.ObjectCollection["rbAll"];

			form.FormClosed += WhenFormClosed;
		}

		protected void WhenFormClosed(object sender, FormClosedEventArgs e)
		{
			// Remove ourselves when the form is closed.
			rsys.Remove(this);
		}

		protected void ShowToday(object sender, EventArgs args)
		{
			DateTime now = DateTime.Now;
			DateTime beginningDate = now.Subtract(new TimeSpan(0, 24, 0, 0));
			DateTime endingDate = now;
			Emit(beginningDate, endingDate);
		}

		protected void ShowLastWeek(object sender, EventArgs args)
		{
			DateTime now = DateTime.Now;
			DateTime beginningDate = now.Subtract(new TimeSpan(7, 0, 0, 0));
			DateTime endingDate = now;
			Emit(beginningDate, endingDate);
		}

		protected void ShowLastMonth(object sender, EventArgs args)
		{
			DateTime now = DateTime.Now;
			DateTime beginningDate = now.Subtract(new TimeSpan(31, 0, 0, 0));
			DateTime endingDate = now;
			Emit(beginningDate, endingDate);
		}

		protected void ShowLastYear(object sender, EventArgs args)
		{
			DateTime now = DateTime.Now;
			DateTime beginningDate = now.Subtract(new TimeSpan(365, 0, 0, 0));
			DateTime endingDate = now;
			Emit(beginningDate, endingDate);
		}

		protected void ShowAll(object sender, EventArgs args)
		{
			DateTime now = DateTime.Now;
			DateTime beginningDate = DateTime.MinValue;
			DateTime endingDate = now;
			Emit(beginningDate, endingDate);
		}

		protected void Emit(DateTime beginningDate, DateTime endingDate)
		{
			CreateCarrier("SearchDateRange", signal =>
			{
				signal.BeginningDate = beginningDate;
				signal.EndingDate = endingDate;
			});
		}
	}
}
