﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using UniRx;

namespace ConfigurableGrowZone
{
    public class StatData
    {
        private readonly Vector2 latLong;

        public StatData(Vector2 latLong)
        {
            this.latLong = latLong;
        }

        public readonly List<SourceMetric> SourceMetrics = new List<SourceMetric>();
        public readonly List<DerivedMetric> DerivedMetrics = new List<DerivedMetric>();
        public readonly StatHistory History = new StatHistory();

        public void AddSourceMetric(SourceMetric metric)
        {
            CreateVolume(metric);

            metric.ValuePushed.Subscribe(dataPoint =>
            {
                History.Save(metric.Key, dataPoint);
            });

            this.SourceMetrics.Add(metric);
        }

        public void AddDerivedMetric(DerivedMetric derivedMetric)
        {
            CreateVolume(derivedMetric);

            derivedMetric.ValuePushed.Subscribe(dataPoint =>
            {
                History.Save(derivedMetric.Key, dataPoint);
            });

            this.DerivedMetrics.Add(derivedMetric);
        }

        public void PersistData()
        {
            foreach(SourceMetric metric in SourceMetrics)
            {
                if(metric is SetSourceMetric) // at the moment only children of SetStatMetric have state
                {
                    SetSourceMetric setStatMetric = (SetSourceMetric)metric;

                    var tempFloatListRef = setStatMetric.GetInternalState().ToList();
                    Scribe_Collections.Look(ref tempFloatListRef, $"{setStatMetric.Key}-partial");

                    setStatMetric.SetInternalState(tempFloatListRef);
                }
            }

            foreach(var kv in History.History)
            {
                var tempDataPoints = kv.Value.DataPoints;
                Scribe_Collections.Look(ref tempDataPoints, kv.Key, LookMode.Deep);
                History.History[kv.Key].DataPoints = tempDataPoints;
            }
        }

        private void CreateVolume(IMetric metric)
        {
            if (!History.ContainsKey(metric.Key))
            {
                History.CreateVolume(metric.Key, new DataVolume(metric.Key, metric.Name, metric.Unit, metric.Domain, latLong));
            }
        }
    }
}
