﻿using DataStoreLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStoreLib.Storage
{
    public interface IStore
    {
        List<MovieEntity> GetMoviesByid(List<string> id);
        List<ReviewEntity> GetReviewsById(List<string> id);

        List<bool> UpdateMoviesById(List<MovieEntity> movies);
        List<bool> UpdateReviewsById(List<ReviewEntity> reviews);
    }

    public static class IStoreHelpers
    {
        public static MovieEntity GetMovieById(this IStore store, string id)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(id));
            var list = new List<string> { id };
            var retList = store.GetMoviesByid(list);

            Debug.Assert(retList.Count == 1);
            return retList[0];
        }

        public static ReviewEntity GetReviewById(this IStore store, string id)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(id));
            var list = new List<string> { id };
            var retList = store.GetReviewsById(list);

            Debug.Assert(retList.Count == 1);
            return retList[0];
        }

        public static bool UpdateMovieById(this IStore store, MovieEntity movie)
        {
            Debug.Assert( movie != null );
            var list = new List<MovieEntity> { movie };
            var retList = store.UpdateMoviesById(list);

            Debug.Assert(retList.Count == 1);
            return retList[0];
        }

        public static bool UpdateReviewById(this IStore store, ReviewEntity review)
        {
            Debug.Assert(review != null);
            var list = new List<ReviewEntity> { review };
            var retList = store.UpdateReviewsById(list);

            Debug.Assert(retList.Count == 1);
            return retList[0];
        }
    }
}