using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Ryujinx.Ava.UI.Helpers
{
#nullable enable
    public static class Commands
    {
        public static RelayCommand Create(Action action)
            => new(action);
        public static RelayCommand CreateConditional(Action action, Func<bool> canExecute)
            => new(action, canExecute);
        
        public static RelayCommand<T> CreateWithArg<T>(Action<T?> action)
            => new(action);
        public static RelayCommand<T> CreateConditionalWithArg<T>(Action<T?> action, Predicate<T?> canExecute)
            => new(action, canExecute);

        public static AsyncRelayCommand CreateAsync(Func<Task> action)
            => new(action, AsyncRelayCommandOptions.None);
        public static AsyncRelayCommand CreateConcurrentAsync(Func<Task> action)
            => new(action, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        public static AsyncRelayCommand CreateSilentFailAsync(Func<Task> action)
            => new(action, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        
        public static AsyncRelayCommand<T> CreateWithArgAsync<T>(Func<T?, Task> action)
            => new(action, AsyncRelayCommandOptions.None);
        public static AsyncRelayCommand<T> CreateConcurrentWithArgAsync<T>(Func<T?, Task> action)
            => new(action, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        public static AsyncRelayCommand<T> CreateSilentFailWithArgAsync<T>(Func<T?, Task> action)
            => new(action, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

        public static AsyncRelayCommand CreateConditionalAsync(Func<Task> action, Func<bool> canExecute)
            => new(action, canExecute, AsyncRelayCommandOptions.None);
        public static AsyncRelayCommand CreateConcurrentConditionalAsync(Func<Task> action, Func<bool> canExecute)
            => new(action, canExecute, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        public static AsyncRelayCommand CreateSilentFailConditionalAsync(Func<Task> action, Func<bool> canExecute)
            => new(action, canExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        
        public static AsyncRelayCommand<T> CreateConditionalWithArgAsync<T>(Func<T?, Task> action, Predicate<T?> canExecute)
            => new(action, canExecute, AsyncRelayCommandOptions.None);
        public static AsyncRelayCommand<T> CreateConcurrentConditionalWithArgAsync<T>(Func<T?, Task> action, Predicate<T?> canExecute)
            => new(action, canExecute, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        public static AsyncRelayCommand<T> CreateSilentFailConditionalWithArgAsync<T>(Func<T?, Task> action, Predicate<T?> canExecute)
            => new(action, canExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }
}
