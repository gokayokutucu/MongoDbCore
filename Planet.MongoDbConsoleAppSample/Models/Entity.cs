using System;

namespace Planet.MongoDbConsoleAppSample.Models {
    public abstract class Entity {
        //int? _requestedHashCode;
        string _id;
        public virtual string Id {
            get {
                return _id;
            }
            protected set {
                _id = value;
            }
        }

        string _createdBy;
        public virtual string CreatedBy {
            get {
                return _createdBy;
            }
            protected set {
                _createdBy = value;
            }
        }

        string _modifiedBy;
        public string ModifiedBy {
            get {
                return _modifiedBy;
            }
            protected set {
                _modifiedBy = value;
            }
        }

        DateTime? _createdDate;
        public Nullable<DateTime> CreatedDate {
            get {
                return _createdDate;
            }
            protected set {
                _createdDate = value;
            }
        }
        DateTime? _modifiedDate;
        public Nullable<DateTime> ModifiedDate {
            get {
                return _modifiedDate;
            }
            protected set {
                _modifiedDate = value;
            }
        }
        public bool IsDeleted { get; set; }
    }
}