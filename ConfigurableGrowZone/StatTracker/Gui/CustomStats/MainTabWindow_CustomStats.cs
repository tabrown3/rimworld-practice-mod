﻿using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableGrowZone
{
    [StaticConstructorOnStartup]
    public class MainTabWindow_CustomStats : MainTabWindow
    {
        private List<TabRecord> leftTabs = new List<TabRecord>();
        private List<TabRecord> rightTabs = new List<TabRecord>();

        private readonly TrackersTab TrackersTab = new TrackersTab();
        private readonly SignalsTab SignalsTab = new SignalsTab();
        private readonly MetricsTab MetricsTab = new MetricsTab();
        private readonly DerivedTab DerivedTab = new DerivedTab();

        private ITabDrawable<CustomStatsTab> leftTabActive;
        private ITabDrawable<CustomStatsTab> rightTabActive;

        public override void PreOpen()
        {
            base.PreOpen();

            leftTabActive = TrackersTab;
            rightTabActive = MetricsTab;

            leftTabs.Clear();
            rightTabs.Clear();

            leftTabs.Add(new TabRecord("Trackers", () => leftTabActive = TrackersTab, () => leftTabActive == TrackersTab));
            leftTabs.Add(new TabRecord("Signals", () => leftTabActive = SignalsTab, () => leftTabActive == SignalsTab));

            rightTabs.Add(new TabRecord("Metrics", () => rightTabActive = MetricsTab, () => rightTabActive == MetricsTab));
            rightTabs.Add(new TabRecord("Derived", () => rightTabActive = DerivedTab, () => rightTabActive == DerivedTab));
        }
        public override void DoWindowContents(Rect inRect)
        {
            base.DoWindowContents(inRect);

            Rect leftTabRect = inRect.AtZero();
            leftTabRect.width = leftTabRect.width/2f - 5f;
            leftTabRect.yMin += 45f;
            TabDrawer.DrawTabs(leftTabRect, leftTabs);

            DrawPane(leftTabRect, leftTabActive);

            Rect rightTabRect = inRect.AtZero();
            rightTabRect.width = rightTabRect.width/2f - 5f;
            rightTabRect.x = rightTabRect.width + 10f;
            rightTabRect.yMin += 45f;
            TabDrawer.DrawTabs(rightTabRect, rightTabs);
            
            DrawPane(rightTabRect, rightTabActive);
        }

        private void DrawPane(Rect outerPane, ITabDrawable<CustomStatsTab> activeTab)
        {
            using (new GuiGroup(outerPane))
            {
                outerPane = outerPane.AtZero();
                Widgets.DrawBox(outerPane);

                Rect innerPane = new Rect(outerPane);
                innerPane.width -= 10f;
                innerPane.height -= 10f;

                activeTab.DrawTab(innerPane);
            }
        }
    }
}
