using System;
using System.Collections.ObjectModel;

namespace JogoApi.Interfaces
{
    public interface IDAO<T>: IDisposable where T: class, new()
    {
        //Create
        T Create (T model);
        //Find By Id
        T FindByID(params Object[] keys);
        //Get all 
        Collection<T> GetAll();

        //Get all for friends

         Collection<T> GetAllData();
        //Update
        void Update(T model);
        //Delete
        bool Delete(T model);


    }
}