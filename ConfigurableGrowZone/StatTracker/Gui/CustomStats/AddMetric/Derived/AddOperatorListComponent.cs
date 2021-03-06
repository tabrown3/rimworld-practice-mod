﻿using ConfigurableGrowZone.StatTracker.Gui.CustomStats.AddMetric.Derived;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Verse;

namespace ConfigurableGrowZone
{
    public class AddOperatorListComponent
    {
        private readonly List<AddOperatorRowComponent> rows = new List<AddOperatorRowComponent>();
        private IObservable<bool> rowsBecameValid;

        public readonly AddOperatorListModel Model;

        public AddOperatorListComponent(AddOperatorOptionsManager optionsManager, AddOperatorListModel model)
        {
            Model = model;
            AddRow(optionsManager);
        }

        public RectConnector Draw(Rect inRect)
        {
            return new RectStacker(inRect)
                .Then(u =>
                {
                    return new RectSpanner(u)
                        .Then(v => StatWidgets.DrawSectionHeader(v, "Operator"))
                        .ThenGap(50f)
                        .Then(v => StatWidgets.DrawSectionHeader(v, "Tracker"))
                        .ThenGap(50f)
                        .Then(v => StatWidgets.DrawSectionHeader(v, "Metric"));
                })
                .ThenForEach(rows, (u, row, ind) => row.Draw(u));
        }

        private void AddRow(AddOperatorOptionsManager optionsManager)
        {
            Model.Rows.Add(new AddOperatorRowModel());
            var newRow = new AddOperatorRowComponent(optionsManager, Model.Rows.Last());
            rows.Add(newRow);

            rowsBecameValid = RowsBecameValidFactory();
            rowsBecameValid.Subscribe(u =>
            {
                AddRow(optionsManager);
            });
        }

        private IObservable<bool> RowsBecameValidFactory()
        {
            bool wasValid = false; // can start false because after a new row is added, it will always be invalid

            // observable that emits when the list's rows go from "not all being valid" to "all being valid"
            return rows.ToObservable()
                .SelectMany(u => u.RowBecameValid)
                .Select(u => rows.All(v => v.Model.IsValid()))
                .Where(isValid =>
                {
                    bool validityStateChanged = isValid != wasValid;
                    wasValid = isValid;
                    return validityStateChanged && isValid;
                }
            );
        }
    }
}
