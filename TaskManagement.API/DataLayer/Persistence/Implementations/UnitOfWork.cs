using TaskManagement.Models;
using TaskManagement.Persistence.Implementations;
using Microsoft.Practices.Unity;
using System;
using TaskManagement.API.DataLayer.Models;

namespace TaskManagement.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        [Dependency]
        public Entities Context { get; set; }

        private GenericRepository<Task> _tasksRepository;
        public GenericRepository<Task> TaskRepository
        {
            get
            {
                if (this._tasksRepository == null)
                {
                    this._tasksRepository = new GenericRepository<Task>();
                    this._tasksRepository.Context = Context;
                }
                return this._tasksRepository;
            }
        }

        private GenericRepository<UserProfile> _usersRepository;
        public GenericRepository<UserProfile> UserProfilesRepository
        {
            get
            {
                if (this._usersRepository == null)
                {
                    this._usersRepository = new GenericRepository<UserProfile>();
                    this._usersRepository.Context = Context;
                }
                return this._usersRepository;
            }
        }

        private GenericRepository<UsersInTask> _usersInTasksRepository;
        public GenericRepository<UsersInTask> UsersInTasksRepository
        {
            get
            {
                if (this._usersInTasksRepository == null)
                {
                    this._usersInTasksRepository = new GenericRepository<UsersInTask>();
                    this._usersInTasksRepository.Context = Context;
                }
                return this._usersInTasksRepository;
            }
        }

        public void Save()
        {
            Context.SaveChanges();
        }




        public void Dispose()
        {
            Context.Dispose();
        }
    }
}