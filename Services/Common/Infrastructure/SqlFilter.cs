/*
FilterExpression :Filter | InFilter | ANDFilter | ORFilter
Filter :Filter Name & Sql Operator & Filter Value 
InFilter :Filter Name & 'IN' & {Filter Value}+
ANDFilter :'(' & FilterExpression & 'AND' & {FilterExpression}+ & ')'
ORFilter :'(' & FilterExpression & 'OR' & {FilterExpression}+ & ')'
* */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Infrastructure
{
    public enum SqlOperators
    {
        Greater,
        Less,
        Equal,
        StartsLike,
        EndsLike,
        Like,
        NotLike,
        LessOrEqual,
        GreaterOrEqual,
        NotEqual
    }

    public class Expression
    {
        public string Property { get; set; }
        public SqlOperators Operation { get; set; }
        public object Value { get; set; }
        public static IEnumerable<Expression> StringToExpressions(string param)
        {
            if (!string.IsNullOrWhiteSpace(param))
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                return JsonSerializer.Deserialize<IEnumerable<Expression>>(param, options);
            }
            return null;
        }
    }
    public interface IFilter
    {
        string FilterString
        {
            get;
        }
        Dictionary<string, object> Params { get; }
    }
    public class Filter : IFilter
    {
        private string _property;
        private SqlOperators _operation;
        private object _origvalue;
        private object _value;
        private string _param;

        public Dictionary<string, object> Params
        {
            get
            {
                var dict = new Dictionary<string, object>();
                dict.Add(_param, _value);
                return dict;
            }
        }
        public Filter(string strFilterName, SqlOperators sqlOperator, object strFilterValue)
        {
            _property = strFilterName;
            _operation = sqlOperator;
            _origvalue = ConvertToType(strFilterValue);
            _param = GetParamName();
        }

        private object ConvertToType(object toConv)
        {
            if (toConv is JsonElement)
                return ReadValue((JsonElement)toConv);
            return toConv;
        }
        private object ReadValue(JsonElement jsonElement)
        {
            object result = null;
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.String:
                    //TODO: Missing Datetime&Bytes Convert
                    result = jsonElement.GetString();
                    break;
                case JsonValueKind.Number:
                    //TODO: more num type
                    result = 0;
                    if (jsonElement.TryGetInt64(out long l))
                    {
                        result = l;
                    }
                    break;
                case JsonValueKind.True:
                    result = true;
                    break;
                case JsonValueKind.False:
                    result = false;
                    break;
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    result = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
        private string GetParamName()
        {
            return _property.Replace(" ", "").Replace("+", "").Replace("'", "").Trim() + (new Random().Next(10000)).ToString();
        }
        public string FilterString
        {
            get
            {
                string strFilter = "";
                _value = _origvalue;
                //_value = _value.Replace("'", "''");
                switch (_operation)
                {
                    case SqlOperators.Greater:
                        strFilter = $" {_property} > @{_param} ";
                        break;
                    case SqlOperators.Less:
                        strFilter = $" {_property} < @{_param} ";
                        break;
                    case SqlOperators.Equal:
                        strFilter = $" {_property} = @{_param} ";
                        break;
                    case SqlOperators.LessOrEqual:
                        strFilter = $" {_property} <= @{_param} ";
                        break;
                    case SqlOperators.GreaterOrEqual:
                        strFilter = $" {_property} >= @{_param} ";
                        break;
                    case SqlOperators.NotEqual:
                        strFilter = $" {_property} <> @{_param} ";
                        break;
                    case SqlOperators.StartsLike:
                        _value = $"{_origvalue}%";
                        strFilter = $" {_property} LIKE @{_param} ";
                        break;
                    case SqlOperators.EndsLike:
                        _value = $"%{_origvalue}";
                        strFilter = $" {_property} LIKE @{_param} ";
                        break;
                    case SqlOperators.Like:
                        _value = $"%{_origvalue}%";
                        strFilter = $" {_property} LIKE @{_param} ";
                        break;
                    case SqlOperators.NotLike:
                        strFilter = $" {_property} NOT LIKE @{_param} ";
                        break;
                    default:
                        throw new Exception("This operator type is not supported");
                }
                return strFilter;
            }
        }
    }
    //public class INFilter : IFilter
    //{
    //    private string m_strFilterName;
    //    private StringCollection m_strColFilterValues;
    //    public INFilter(string strFilterName, StringCollection strColValues)
    //    {
    //        m_strFilterName = strFilterName;
    //        m_strColFilterValues = strColValues;
    //    }

    //    public string FilterString
    //    {
    //        get
    //        {
    //            string strFilter = "";
    //            if (m_strColFilterValues.Count > 0)
    //            {
    //                for (int i = 0; i < m_strColFilterValues.Count - 1; i++)
    //                {
    //                    strFilter += "'" + m_strColFilterValues[i].ToString() + "'" + ",";
    //                }
    //                strFilter += "'" + m_strColFilterValues[m_strColFilterValues.Count - 1].ToString() + "'";
    //                strFilter = m_strFilterName + " IN(" + strFilter + ")";
    //            }
    //            return strFilter;
    //        }
    //    }
    //}
    public class ANDFilter : IFilter
    {
        private List<IFilter> m_filterList = new List<IFilter>();
        public ANDFilter(IFilter filterLeft, IFilter filterRight)
        {
            if (filterLeft != null && filterLeft.Params.Any())
                m_filterList.Add(filterLeft);
            if (filterRight != null && filterRight.Params.Any())
                m_filterList.Add(filterRight);
        }

        public ANDFilter(List<IFilter> filterList)
        {
            m_filterList = filterList;
        }
        public string FilterString
        {
            get
            {
                string strFilter = "";
                if (m_filterList.Count > 0)
                {
                    for (int i = 0; i < m_filterList.Count - 1; i++)
                    {
                        strFilter += m_filterList[i].FilterString + " AND ";
                    }
                    strFilter += m_filterList[m_filterList.Count - 1].FilterString;
                    strFilter = "(" + strFilter + ")";
                }
                return strFilter;
            }
        }
        public Dictionary<string, object> Params
        {
            get
            {
                Dictionary<string, object> ret = new Dictionary<string, object>();
                foreach (var filter in m_filterList)
                {
                    foreach (var j in filter.Params)
                        ret[j.Key] = j.Value;
                }
                return ret;
            }
        }
    }
    public class ORFilter : IFilter
    {
        private List<IFilter> m_filterList = new List<IFilter>();
        public ORFilter(IFilter filterLeft, IFilter filterRight)
        {
            m_filterList.Add(filterLeft);
            m_filterList.Add(filterRight);
        }
        public ORFilter(List<IFilter> filterList)
        {
            m_filterList = filterList;
        }

        public string FilterString
        {
            get
            {
                string strFilter = "";
                if (m_filterList.Count > 0)
                {
                    for (int i = 0; i < m_filterList.Count - 1; i++)
                    {
                        strFilter += m_filterList[i].FilterString + " OR ";
                    }
                    strFilter += m_filterList[m_filterList.Count - 1].FilterString;
                    strFilter = "(" + strFilter + ")";
                }
                return strFilter;
            }
        }
        public Dictionary<string, object> Params
        {
            get
            {
                Dictionary<string, object> ret = new Dictionary<string, object>();
                foreach (var filter in m_filterList)
                {
                    foreach (var j in filter.Params)
                        ret[j.Key] = j.Value;
                }
                return ret;
            }
        }
    }
}