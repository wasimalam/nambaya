using Common.DataAccess.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace Common.DataAccess.Models
{
    public class BaseModel : IBaseModel
    {
        private DBState _dataState = DBState.Unchanged;
        [DapperIgnore]
        public DBState DataState
        {
            get { return _dataState; }
            set
            {
                _dataState = value;
            }
        }

        public virtual long ID { get; set; }

        private string _createdby;

        public virtual string CreatedBy
        {
            get { return _createdby; }
            set
            {
                if (_createdby != value)
                {
                    _createdby = value;
                    SetField(ref _createdby, value, "CreatedBy");
                }
            }
        }

        private string _updatedby;

        public virtual string UpdatedBy
        {
            get { return _updatedby; }
            set
            {
                if (_updatedby != value)
                {
                    _updatedby = value;
                    SetField(ref _updatedby, value, "UpdatedBy");
                }
            }
        }

        private DateTime? _createdon;

        public virtual DateTime? CreatedOn
        {
            get { return _createdon; }
            set
            {
                if (_createdon != value)
                {
                    _createdon = value;
                    SetField(ref _createdon, value, "CreatedOn");
                }
            }
        }

        private DateTime? _updatedon;

        public virtual DateTime? UpdatedOn
        {
            get { return _updatedon; }
            set
            {
                if (_updatedon != value)
                {
                    _updatedon = value;
                    SetField(ref _updatedon, value, "UpdatedOn");
                }
            }
        }

        public T GetOrSetPropValue<T>(Object obj, String name, bool isGet, T value)
        {
            if (isGet)
            {
                Object retval = GetPropValue(obj, name);
                if (retval == null) { return default(T); }

                return (T)retval;
            }
            else
            {
                SetPropValue(obj, name, value);
                return default(T);
            }
        }

        private Object GetPropValue(Object obj, String name)
        {

            Type type = obj.GetType();
            PropertyInfo c = type.GetProperties().FirstOrDefault(p => p.Name.Contains(name));
            //c.SetValue(obj, Value);
            return c?.GetValue(obj);
        }

        private void SetPropValue(Object obj, String name, object value)
        {

            Type type = obj.GetType();
            PropertyInfo c = type.GetProperties().FirstOrDefault(p => p.Name.Contains(name));
            c.SetValue(obj, value);
            //return c.GetValue(obj);
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            //if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            ManageTheRowState(true);
            return true;
        }

        private void ManageTheRowState(bool change)
        {
            if (this.DataState == DBState.Unchanged && change)
                this.DataState = DBState.Add;
            else if (this.DataState == DBState.None && change)
                this.DataState = DBState.Update;
            else
                this.DataState = this.DataState;
        }

        public T Clone<T>(T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)inst?.Invoke(obj, null);
        }
    }
}
