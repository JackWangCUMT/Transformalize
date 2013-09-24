﻿#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Linq;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Infrastructure.Introspection;
using Transformalize.Libs.Ninject.Infrastructure.Language;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Planning.Targets;
using Transformalize.Libs.Ninject.Syntax;

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     Provides a root for the fluent syntax associated with an <see cref="BindingConfiguration" />.
    /// </summary>
    /// <typeparam name="T">The implementation type of the built binding.</typeparam>
    public class BindingConfigurationBuilder<T> : IBindingConfigurationSyntax<T>
    {
        /// <summary>
        ///     The names of the services added to the exceptions.
        /// </summary>
        private readonly string serviceNames;

        /// <summary>
        ///     Initializes a new instance of the BindingBuilder&lt;T&gt; class.
        /// </summary>
        /// <param name="bindingConfiguration">The binding configuration to build.</param>
        /// <param name="serviceNames">The names of the configured services.</param>
        /// <param name="kernel">The kernel.</param>
        public BindingConfigurationBuilder(IBindingConfiguration bindingConfiguration, string serviceNames, IKernel kernel)
        {
            Ensure.ArgumentNotNull(bindingConfiguration, "bindingConfiguration");
            Ensure.ArgumentNotNull(kernel, "kernel");
            BindingConfiguration = bindingConfiguration;
            Kernel = kernel;
            this.serviceNames = serviceNames;
        }

        /// <summary>
        ///     Gets the binding being built.
        /// </summary>
        public IBindingConfiguration BindingConfiguration { get; private set; }

        /// <summary>
        ///     Gets the kernel.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        ///     Indicates that the binding should be used only for requests that support the specified condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> When(Func<IRequest, bool> condition)
        {
            BindingConfiguration.Condition = condition;
            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto<TParent>()
        {
            return WhenInjectedInto(typeof (TParent));
        }

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(Type parent)
        {
            if (parent.IsGenericTypeDefinition)
            {
                if (parent.IsInterface)
                {
                    BindingConfiguration.Condition = r =>
                                                     r.Target != null &&
                                                     r.Target.Member.ReflectedType.GetInterfaces().Any(i =>
                                                                                                       i.IsGenericType &&
                                                                                                       i.GetGenericTypeDefinition() == parent);
                }
                else
                {
                    BindingConfiguration.Condition = r =>
                                                     r.Target != null &&
                                                     r.Target.Member.ReflectedType.GetAllBaseTypes().Any(i =>
                                                                                                         i.IsGenericType &&
                                                                                                         i.GetGenericTypeDefinition() == parent);
                }
            }
            else
            {
                BindingConfiguration.Condition = r => r.Target != null && parent.IsAssignableFrom(r.Target.Member.ReflectedType);
            }

            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     Types that derive from the specified type are considered as valid targets.
        /// </summary>
        /// <param name="parents">The type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedInto(params Type[] parents)
        {
            BindingConfiguration.Condition = r =>
                                                 {
                                                     foreach (var parent in parents)
                                                     {
                                                         var matches = false;
                                                         if (parent.IsGenericTypeDefinition)
                                                         {
                                                             if (parent.IsInterface)
                                                             {
                                                                 matches =
                                                                     r.Target != null &&
                                                                     r.Target.Member.ReflectedType.GetInterfaces().Any(i =>
                                                                                                                       i.IsGenericType &&
                                                                                                                       i.GetGenericTypeDefinition() == parent);
                                                             }
                                                             else
                                                             {
                                                                 matches =
                                                                     r.Target != null &&
                                                                     r.Target.Member.ReflectedType.GetAllBaseTypes().Any(i =>
                                                                                                                         i.IsGenericType &&
                                                                                                                         i.GetGenericTypeDefinition() == parent);
                                                             }
                                                         }
                                                         else
                                                         {
                                                             matches = r.Target != null && parent.IsAssignableFrom(r.Target.Member.ReflectedType);
                                                         }

                                                         if (matches) return true;
                                                     }

                                                     return false;
                                                 };

            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     The type must match exactly the specified type. Types that derive from the specified type
        ///     will not be considered as valid target.
        /// </summary>
        /// <typeparam name="TParent">The type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto<TParent>()
        {
            return WhenInjectedExactlyInto(typeof (TParent));
        }

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     The type must match exactly the specified type. Types that derive from the specified type
        ///     will not be considered as valid target.
        /// </summary>
        /// <param name="parent">The type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto(Type parent)
        {
            if (parent.IsGenericTypeDefinition)
            {
                BindingConfiguration.Condition = r =>
                                                 r.Target != null &&
                                                 r.Target.Member.ReflectedType.IsGenericType &&
                                                 parent == r.Target.Member.ReflectedType.GetGenericTypeDefinition();
            }
            else
            {
                BindingConfiguration.Condition = r => r.Target != null && r.Target.Member.ReflectedType == parent;
            }
            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only for injections on the specified type.
        ///     The type must match exactly the specified type. Types that derive from the specified type
        ///     will not be considered as valid target.
        ///     Should match at least one of the specified targets
        /// </summary>
        /// <param name="parents">The types.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenInjectedExactlyInto(params Type[] parents)
        {
            BindingConfiguration.Condition = r =>
                                                 {
                                                     foreach (var parent in parents)
                                                     {
                                                         var matches = false;
                                                         if (parent.IsGenericTypeDefinition)
                                                         {
                                                             matches =
                                                                 r.Target != null &&
                                                                 r.Target.Member.ReflectedType.IsGenericType &&
                                                                 parent == r.Target.Member.ReflectedType.GetGenericTypeDefinition();
                                                         }
                                                         else
                                                         {
                                                             matches = r.Target != null && r.Target.Member.ReflectedType == parent;
                                                         }

                                                         if (matches) return true;
                                                     }

                                                     return false;
                                                 };

            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only when the class being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenClassHas(typeof (TAttribute));
        }

        /// <summary>
        ///     Indicates that the binding should be used only when the member being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenMemberHas(typeof (TAttribute));
        }

        /// <summary>
        ///     Indicates that the binding should be used only when the target being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas<TAttribute>() where TAttribute : Attribute
        {
            return WhenTargetHas(typeof (TAttribute));
        }

        /// <summary>
        ///     Indicates that the binding should be used only when the class being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenClassHas(Type attributeType)
        {
            if (!typeof (Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(serviceNames, "WhenClassHas", attributeType));
            }

            BindingConfiguration.Condition = r => r.Target != null && r.Target.Member.ReflectedType.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only when the member being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenMemberHas(Type attributeType)
        {
            if (!typeof (Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(serviceNames, "WhenMemberHas", attributeType));
            }

            BindingConfiguration.Condition = r => r.Target != null && r.Target.Member.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only when the target being injected has
        ///     an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The type of attribute.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenTargetHas(Type attributeType)
        {
            if (!typeof (Attribute).IsAssignableFrom(attributeType))
            {
                throw new InvalidOperationException(ExceptionFormatter.InvalidAttributeTypeUsedInBindingCondition(serviceNames, "WhenTargetHas", attributeType));
            }

            BindingConfiguration.Condition = r => r.Target != null && r.Target.HasAttribute(attributeType);

            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only when the service is being requested
        ///     by a service bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenParentNamed(string name)
        {
            String.Intern(name);
            BindingConfiguration.Condition = r => r.ParentContext != null && string.Equals(r.ParentContext.Binding.Metadata.Name, name, StringComparison.Ordinal);
            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        [Obsolete("Use WhenAnyAncestorNamed(string name)")]
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAnchestorNamed(string name)
        {
            return WhenAnyAncestorNamed(name);
        }

        /// <summary>
        ///     Indicates that the binding should be used only when any ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorNamed(string name)
        {
            return WhenAnyAncestorMatches(ctx => ctx.Binding.Metadata.Name == name);
        }

        /// <summary>
        ///     Indicates that the binding should be used only when no ancestor is bound with the specified name.
        /// </summary>
        /// <param name="name">The name to expect.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorNamed(string name)
        {
            return WhenNoAncestorMatches(ctx => ctx.Binding.Metadata.Name == name);
        }

        /// <summary>
        ///     Indicates that the binding should be used only when any ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorMatches(Predicate<IContext> predicate)
        {
            BindingConfiguration.Condition = r => DoesAnyAncestorMatch(r, predicate);
            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be used only when no ancestor matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorMatches(Predicate<IContext> predicate)
        {
            BindingConfiguration.Condition = r => !DoesAnyAncestorMatch(r, predicate);
            return this;
        }

        /// <summary>
        ///     Indicates that the binding should be registered with the specified name. Names are not
        ///     necessarily unique; multiple bindings for a given service may be registered with the same name.
        /// </summary>
        /// <param name="name">The name to give the binding.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> Named(string name)
        {
            string.Intern(name);
            BindingConfiguration.Metadata.Name = name;
            return this;
        }

        /// <summary>
        ///     Indicates that only a single instance of the binding should be created, and then
        ///     should be re-used for all subsequent requests.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InSingletonScope()
        {
            BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Singleton;
            return this;
        }

        /// <summary>
        ///     Indicates that instances activated via the binding should not be re-used, nor have
        ///     their lifecycle managed by Ninject.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InTransientScope()
        {
            BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Transient;
            return this;
        }

        /// <summary>
        ///     Indicates that instances activated via the binding should be re-used within the same thread.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InThreadScope()
        {
            BindingConfiguration.ScopeCallback = StandardScopeCallbacks.Thread;
            return this;
        }

        /// <summary>
        ///     Indicates that instances activated via the binding should be re-used as long as the object
        ///     returned by the provided callback remains alive (that is, has not been garbage collected).
        /// </summary>
        /// <param name="scope">The callback that returns the scope.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingNamedWithOrOnSyntax<T> InScope(Func<IContext, object> scope)
        {
            BindingConfiguration.ScopeCallback = scope;
            return this;
        }

        /// <summary>
        ///     Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, object value)
        {
            BindingConfiguration.Parameters.Add(new ConstructorArgument(name, value));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback)
        {
            BindingConfiguration.Parameters.Add(new ConstructorArgument(name, callback));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget, object> callback)
        {
            BindingConfiguration.Parameters.Add(new ConstructorArgument(name, callback));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, object value)
        {
            BindingConfiguration.Parameters.Add(new PropertyValue(name, value));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback)
        {
            BindingConfiguration.Parameters.Add(new PropertyValue(name, callback));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback)
        {
            BindingConfiguration.Parameters.Add(new PropertyValue(name, callback));
            return this;
        }

        /// <summary>
        ///     Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithParameter(IParameter parameter)
        {
            BindingConfiguration.Parameters.Add(parameter);
            return this;
        }

        /// <summary>
        ///     Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWithOrOnSyntax<T> WithMetadata(string key, object value)
        {
            BindingConfiguration.Metadata.Set(key, value);
            return this;
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation(Action<T> action)
        {
            return OnActivation<T>(action);
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation<TImplementation>(Action<TImplementation> action)
        {
            BindingConfiguration.ActivationActions.Add((context, instance) => action((TImplementation) instance));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation(Action<IContext, T> action)
        {
            return OnActivation<T>(action);
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are activated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnActivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            BindingConfiguration.ActivationActions.Add((context, instance) => action(context, (TImplementation) instance));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation(Action<T> action)
        {
            return OnDeactivation<T>(action);
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<TImplementation> action)
        {
            BindingConfiguration.DeactivationActions.Add((context, instance) => action((TImplementation) instance));
            return this;
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation(Action<IContext, T> action)
        {
            return OnDeactivation<T>(action);
        }

        /// <summary>
        ///     Indicates that the specified callback should be invoked when instances are deactivated.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="action">The action callback.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingOnSyntax<T> OnDeactivation<TImplementation>(Action<IContext, TImplementation> action)
        {
            BindingConfiguration.DeactivationActions.Add((context, instance) => action(context, (TImplementation) instance));
            return this;
        }

        private static bool DoesAnyAncestorMatch(IRequest request, Predicate<IContext> predicate)
        {
            var parentContext = request.ParentContext;
            if (parentContext == null)
            {
                return false;
            }

            return
                predicate(parentContext) ||
                DoesAnyAncestorMatch(parentContext.Request, predicate);
        }
    }
}