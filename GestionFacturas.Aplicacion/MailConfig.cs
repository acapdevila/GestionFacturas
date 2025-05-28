namespace Smtp;

internal record MailConfig(
    string Host, 
    int Port, 
    bool UseSslOrTls, 
    string UserName, 
    string Password);