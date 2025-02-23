﻿namespace Movies.Api
{
    public static class ApiEndpoints
    {
        private const string ApiBase = "/api";

        public static class Movies
        {
            private const string Base = $"{ApiBase}/movies";

            public const string Create = Base;
            public const string GetAll = Base;
            public const string GetById = Base + "/{id}";
            public const string Update = Base + "/{id}";
            public const string Delete = Base + "/{id}";



        }
    }
}
