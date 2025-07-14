namespace conj_ai.Models;

public class ConfigurationException(string Message) : Exception(Message);

public class UnauthorizedException(string Message) : Exception(Message);