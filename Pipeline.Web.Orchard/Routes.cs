﻿#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Pipeline.Web.Orchard {

    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                RouteDescriptorWithId("Api", "Cfg"),
                RouteDescriptorWithId("Api", "Check"),
                RouteDescriptorWithId("Api", "Run"),

                RouteDescriptor("File","Upload"),
                RouteDescriptorWithId("File", "Download"),
                RouteDescriptorWithId("File", "Delete"),
                RouteDescriptorWithId("File", "View"),
                RouteDescriptorWithTagFilter("File", "List"),

                RouteDescriptorWithId("Cfg", "Report"),
                RouteDescriptorWithId("Cfg", "Download"),
                RouteDescriptorWithTagFilter("Cfg", "List"),
            };
        }

        private static RouteDescriptor RouteDescriptorWithId(string controller, string action) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + (controller == "Cfg" ? string.Empty : controller + "/") + action + "/{id}",
                    new RouteValueDictionary {
                        {"area", Common.ModuleName },
                        {"controller", controller },
                        {"action", action},
                        {"id", 0}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", Common.ModuleName } },
                    new MvcRouteHandler()
                    )
            };
        }

        private static RouteDescriptor RouteDescriptor(string controller, string action) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + controller + (controller == action ? string.Empty : "/" + action),
                    new RouteValueDictionary {
                        {"area", Common.ModuleName },
                        {"controller", controller },
                        {"action", action}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", Common.ModuleName } },
                    new MvcRouteHandler()
                    )
            };
        }


        private static RouteDescriptor RouteDescriptorWithTagFilter(string controller, string action) {
            return new RouteDescriptor {
                Priority = 11,
                Route = new Route(
                    "Pipeline/" + (controller == "Cfg" ? string.Empty : controller + "/") + action + "/{tagFilter}",
                    new RouteValueDictionary {
                        {"area", Common.ModuleName },
                        {"controller", controller },
                        {"action", action},
                        {"tagFilter", Common.AllTag}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary { { "area", Common.ModuleName } },
                    new MvcRouteHandler()
                    )
            };
        }

    }
}