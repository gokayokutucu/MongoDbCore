using System;

namespace Planet.MongoDbConsoleAppSample.Exceptions {
    public class PlanetDomainException : Exception {
        public PlanetDomainException (string name, string message, Exception ex) : base ($"Service of entity \"{name}\" is failed. {message}", ex) { }
    }
}