﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataStoreLib.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace DataStoreLib.Storage
{
    public abstract class Table
    {
        protected CloudTable _table;
        protected Table(CloudTable table)
        {
            _table = table;
        }

        public virtual IDictionary<string, TEntity> GetItemsById<TEntity>(List<string> ids, string partitionKey = "") where TEntity : DataStoreLib.Models.TableEntity
        {
            Debug.Assert(ids.Count != 0);
            Debug.Assert(_table != null);

            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                partitionKey = GetParitionKey();
            }

            var operationList = new Dictionary<string, TableResult>();
            foreach (var id in ids)
            {
                operationList[id] = _table.Execute(TableOperation.Retrieve<TEntity>(partitionKey, id));
            }
            
            var returnDict = new Dictionary<string, TEntity>();
            int iter = 0;
            foreach (var tableResult in operationList)
            {
                TEntity entity = null;
                if (tableResult.Value.HttpStatusCode != (int)HttpStatusCode.OK)
                {
                    Trace.TraceWarning("Couldn't retrieve info for id {0}", ids[iter]);
                }
                else
                {
                    entity = tableResult.Value.Result as TEntity;
                }
                returnDict.Add(ids[iter], entity);
                iter++;
            }

            return returnDict;
        }

        public virtual IDictionary<ITableEntity, bool> UpdateItemsById(List<ITableEntity> items, string partitionKey = "")
        {
            var returnDict = new Dictionary<ITableEntity, bool>();

            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                partitionKey = GetParitionKey();
            }

            var batchOp = new TableBatchOperation();
            foreach (var item in items)
            {
                batchOp.Insert(item);
            }

            var tableResult = _table.ExecuteBatch(batchOp);

            foreach (var result in tableResult)
            {
                Debug.Assert((result.Result as ITableEntity) != null);
                if (result.HttpStatusCode >= 200 || result.HttpStatusCode < 300)
                {
                    returnDict[result.Result as ITableEntity] = true;
                }
                else
                {
                    returnDict[result.Result as ITableEntity] = false;
                }
            }

            return returnDict;
        }

        protected abstract string GetParitionKey();
    }

    internal class MovieTable : Table
    {
        protected MovieTable(CloudTable table)
            : base(table)
        {
        }

        internal static Table CreateTable(CloudTable table)
        {
            return new MovieTable(table);
        }

        protected override string GetParitionKey()
        {
            return MovieEntity.PARTITION_KEY;
        }
    }

    internal class ReviewTable : Table
    {
        protected ReviewTable(CloudTable table)
            : base(table)
        {
        }

        internal static Table CreateTable(CloudTable table)
        {
            return new ReviewTable(table);
        }

        protected override string GetParitionKey()
        {
            return ReviewEntity.PARTITION_KEY;
        }
    }
}
