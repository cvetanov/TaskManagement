using TaskManagement.Models;
using TaskManagement.Persistence.Implementations;
using Microsoft.Practices.Unity;
using System;
using TaskManagement.API.DataLayer.Models;
using System.Collections;

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

        private GenericRepository<FriendRequest> _friendRequestsRepository;
        public GenericRepository<FriendRequest> FriendRequestsRepository
        {
            get
            {
                if (this._friendRequestsRepository == null)
                {
                    this._friendRequestsRepository = new GenericRepository<FriendRequest>();
                    this._friendRequestsRepository.Context = Context;
                }
                return this._friendRequestsRepository;
            }
        }

        private GenericRepository<Friendship> _friendsRepository;
        public GenericRepository<Friendship> FriendsRepository
        {
            get
            {
                if (this._friendsRepository == null)
                {
                    this._friendsRepository = new GenericRepository<Friendship>();
                    this._friendsRepository.Context = Context;
                }
                return this._friendsRepository;
            }
        }

        private GenericRepository<Comment> _commentsRepository;
        public GenericRepository<Comment> CommentsRepository
        {
            get
            {
                if (this._commentsRepository == null)
                {
                    this._commentsRepository = new GenericRepository<Comment>();
                    this._commentsRepository.Context = Context;
                }
                return this._commentsRepository;
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