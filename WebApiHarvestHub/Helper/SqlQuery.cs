namespace WebApiHarvestHub.Helper
{
    public static class SqlQuery
    {
        public static string SetCondition(string sql, string sql2)
        {
            if (sql == "" && sql2 != "")
                return  sql2;
            else if (sql != "" && sql2 != "")
                return sql + " AND " + sql2;
            else
                return sql;
        }

        public static string SetConditionWitWhere(string sql, string sql2)
        {
            if (sql == "" && sql2 != "")
                return " WHERE " + sql2;
            else if (sql != "" && sql2 != "")
                return sql + " AND " + sql2;
            else
                return sql;
        }

    }
}