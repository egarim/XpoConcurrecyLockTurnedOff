using DevExpress.Xpo;

namespace XpoConcurrencyTurnOff;

public class Department: XPObject
{
       public Department(Session session) : base(session)
        {
        }
        public string Name
        {
            get => GetPropertyValue<string>(nameof(Name));
            set => SetPropertyValue(nameof(Name), value);
        }
   

  
}