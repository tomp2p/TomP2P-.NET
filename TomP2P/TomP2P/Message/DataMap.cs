﻿using System;
using System.Collections.Generic;
using TomP2P.Peers;
using TomP2P.Storage;

namespace TomP2P.Message
{
    public class DataMap : IEquatable<DataMap>
    {
        public Dictionary<Number640, Data> BackingDataMap { get; private set; }
        public Dictionary<Number160, Data> DataMapConvert { get; private set; }

        public Number160 LocationKey { get; private set; }
        public Number160 DomainKey { get; private set; }
        public Number160 VersionKey { get; private set; }

        public bool IsConvertMeta { get; private set; }

        public DataMap(Dictionary<Number640, Data> dataMap)
            : this(dataMap, false)
        { }

        public DataMap(Dictionary<Number640, Data> dataMap, bool isConvertMeta)
        {
            BackingDataMap = dataMap;
            DataMapConvert = null;

            LocationKey = null;
            DomainKey = null;
            VersionKey = null;

            IsConvertMeta = isConvertMeta;
        }

        public DataMap(Number160 locationKey, Number160 domainKey, Number160 versionKey,
            Dictionary<Number160, Data> dataMapConvert)
            : this(locationKey, domainKey, versionKey, dataMapConvert, false)
        { }

        public DataMap(Number160 loactionKey, Number160 domainKey, Number160 versionKey,
            Dictionary<Number160, Data> dataMapConvert, bool isConvertMeta)
        {
            BackingDataMap = null;
            DataMapConvert = dataMapConvert;

            LocationKey = loactionKey;
            DomainKey = domainKey;
            VersionKey = versionKey;

            IsConvertMeta = isConvertMeta;
        }

        public Dictionary<Number640, Data> ConvertToMap640()
        {
            return Convert(this);
        }

        public Dictionary<Number640, Number160> ConvertToHash()
        {
            var result = new Dictionary<Number640, Number160>();

            if (BackingDataMap != null)
            {
                foreach (var data in BackingDataMap)
                {
                    result.Add(data.Key, data.Value.Hash());
                }
            }
            else if (DataMapConvert != null)
            {
                foreach (var data in DataMapConvert)
                {
                    result.Add(new Number640(LocationKey, DomainKey, data.Key, VersionKey), data.Value.Hash());
                }
            }
            return result;
        }

        private static Dictionary<Number640, Data> Convert(DataMap map)
        {
            Dictionary<Number640, Data> dm;
            if (map.DataMapConvert != null)
            {
                dm = new Dictionary<Number640, Data>(map.DataMapConvert.Count);

                foreach (var data in map.DataMapConvert)
                {
                    dm.Add(new Number640(map.LocationKey, map.DomainKey, data.Key, map.VersionKey), data.Value);
                }
            }
            else
            {
                dm = map.BackingDataMap;
            }
            return dm;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return false;
            }
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            return this.Equals(obj as DataMap);
        }

        public bool Equals(DataMap other)
        {
            Dictionary<Number640, Data> dm2 = Convert(this);
            Dictionary<Number640, Data> dm3 = Convert(other);

            bool t1 = Utils.Utils.IsSameSets(dm2.Keys, dm3.Keys); // TODO test
            bool t2 = Utils.Utils.IsSameSets(dm2.Values, dm3.Values);
            return t1 && t2;
        }

        public override int GetHashCode()
        {
            Dictionary<Number640, Data> dataMap = Convert(this);
            return dataMap.GetHashCode();
        }

        /// <summary>
        /// The size of either the datamap with the Number480 as key, or the datamap with the Number160 as key.
        /// </summary>
        public int Size
        {
            get
            {
                if (BackingDataMap != null)
                {
                    return BackingDataMap.Count;
                }
                if (DataMapConvert != null)
                {
                    return DataMapConvert.Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// True, if we have Number160 stored and we need to add the location and domain key.
        /// </summary>
        public bool IsConvert
        {
            get { return DataMapConvert != null; }
        }
    }
}
