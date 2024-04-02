using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace XpoConcurrencyTurnOff;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        
        if(File.Exists("sqlite.db"))
            File.Delete("sqlite.db");
        
        string conn = "XpoProvider=SQLite;Data Source=sqlite.db";
        IDataLayer dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
        using(Session session = new Session(dl)) {
            System.Reflection.Assembly[] assemblies = new System.Reflection.Assembly[] {
                typeof(Department).Assembly,
               
            };
            session.UpdateSchema(assemblies);
            session.CreateObjectTypeRecords(assemblies);
        }
        
        
        
        XpoTypesInfoHelper.GetXpoTypeInfoSource();
        XafTypesInfo.Instance.RegisterEntity(typeof(Department));
        XPObjectSpaceProvider osProvider = new XPObjectSpaceProvider(
           conn,
            null);

       
        IObjectSpace Os1 = osProvider.CreateObjectSpace();
        IObjectSpace Os2 = osProvider.CreateObjectSpace();

        var Department1= Os1.CreateObject<Department>();
        Department1.Name = "Department1";
        Os1.CommitChanges();
        
        var DepartmentFromAnotherOs= Os2.GetObjectsQuery<Department>().FirstOrDefault();
        DepartmentFromAnotherOs.Name = "Department2";
        Os2.CommitChanges();
        
        
        Department1.Name = "Department3";
        Os1.CommitChanges();
        
        
        
        Assert.Pass();
    }
    [Test]
    public void Test2()
    {
        
        if(File.Exists("sqlite.db"))
            File.Delete("sqlite.db");
        
        string conn = "XpoProvider=SQLite;Data Source=sqlite.db";
        IDataLayer dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
        using(Session session = new Session(dl)) {
            System.Reflection.Assembly[] assemblies = new System.Reflection.Assembly[] {
                typeof(Department).Assembly,
               
            };
            session.UpdateSchema(assemblies);
            session.CreateObjectTypeRecords(assemblies);
        }
        
        
        
        XpoTypesInfoHelper.GetXpoTypeInfoSource();
        XafTypesInfo.Instance.RegisterEntity(typeof(Department));
        XPObjectSpaceProvider osProvider = new XPObjectSpaceProvider(
            conn,
            null);

       
        IObjectSpace Os1 = osProvider.CreateObjectSpace();
        IObjectSpace Os2 = osProvider.CreateObjectSpace();

        var Department1= Os1.CreateObject<Department>();
        Department1.Name = "Department1";
        Os1.CommitChanges();
        
        var DepartmentFromAnotherOs= Os2.GetObjectsQuery<Department>().FirstOrDefault();
        DepartmentFromAnotherOs.Name = "Department2";
        Os2.CommitChanges();
        
        
        Department1.Name = "Department3";
        if (!IsChanged(Department1))
        {
            Os1.CommitChanges();
        }
        else
        {
            XPClassInfo ci = Department1.ClassInfo;
            XPMemberInfo mi = ci.OptimisticLockField;

            using (Session s = new Session(Department1.Session.DataLayer))
            {
                //handle optimistic lock https://supportcenter.devexpress.com/ticket/details/s170226/optimisticlocking-enhance-properties-modifications-tracking
                var CurrentObjectNewLockNumber = mi.GetValue(Department1);
                IXPObject ObjectFromDatabase = (IXPObject)s.GetObjectByKey(ci, ci.GetId(Department1));
                var ObjectFromDatabaseLockNumber = mi.GetValue(ObjectFromDatabase);
                //HACK is not necessary to update the lock number if you set the locking options to none before commit
                Department1.Session.LockingOption = LockingOption.None;
                //mi.SetValue(Department1, ObjectFromDatabaseLockNumber);
                CurrentObjectNewLockNumber = mi.GetValue(Department1);
               
                Os1.CommitChanges();
            }
        }




        Assert.Pass();
    }
    public bool IsChanged(IXPObject obj)  
    {  
        XPClassInfo ci = obj.ClassInfo;  
        XPMemberInfo mi = ci.OptimisticLockField;  
        if (mi == null) return false;  
        using (Session s = new Session(obj.DataLayer))  
        {  
            IXPObject obj2 = (IXPObject)s.GetObjectByKey(ci, ci.GetId(obj)); 
            var Object1Lock=mi.GetValue(obj);
            var Object2Lock=mi.GetValue(obj2);
            var IsChanged=!Equals(Object1Lock, Object2Lock);
            return IsChanged;
        }  
    }  
}