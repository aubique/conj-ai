namespace conj_ai.Models;

public class ConfigurationException(string Message) : Exception(Message);

public class UnauthorizedAiException(string Message) : Exception(Message);