﻿using System.Reflection;
using Autofac;
using Cqrs.Hotel.Command.Sagas;
using Cqrs.Hotel.Data;
using Cqrs.Hotel.Data.Repositories;
using Cqrs.Hotel.Data.Repositories.Interfaces;
using Cqrs.Hotel.Domain;
using Cqrs.Hotel.Infraestructure;
using MediatR;
using MediatR.Pipeline;

namespace Cqrs.Hotel
{
    public class Hotel
    {

        public ILifetimeScope Initilize()
        {

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            var mediatrOpenTypes = new[] { typeof(IRequestHandler<,>), typeof(INotificationHandler<>), };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(typeof(DomainEvent).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
                builder
                    .RegisterAssemblyTypes(typeof(HandlerLeaveRoom).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });


            builder.RegisterType<MemoryBus>().As<IBus>();
            builder.RegisterType<DomainEventStore>().As<IDomainEventStore>();
            builder.RegisterType<ClientRepository>().As<IClientRepository>();
            builder.RegisterType<BookingRepository>().As<IBookingRepository>();
            builder.RegisterType<DomainEventRepository>().As<IDomainEventRepository>();

          

            var container = builder.Build();

            return container.BeginLifetimeScope();
        }
    }
}
