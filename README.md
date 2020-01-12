[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Divergic/Divergic.Logging/blob/master/LICENSE)&nbsp;&nbsp;&nbsp;[![Nuget](https://img.shields.io/nuget/v/Divergic.Logging.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Divergic.Logging.svg)](https://www.nuget.org/packages/Divergic.Logging)

[![Actions Status](https://github.com/divergic/Divergic.Logging/workflows/CI/badge.svg)](https://github.com/divergic/Divergic.Logging/actions)&nbsp;[![Coverage Status](https://coveralls.io/repos/github/Divergic/Divergic.Logging/badge.svg?branch=master)](https://coveralls.io/github/Divergic/Divergic.Logging?branch=master)

# Introduction    

The Divergic.Logging packages provide ```ILogger``` and ```ILoggerFactory```  extension methods for adding context data when logging exceptions.

# Installation

There are two NuGet packages to give flexibility around how applications can use this feature.

- ```Install-Package Divergic.Logging``` [on NuGet.org](https://www.nuget.org/packages/Divergic.Logging) contains the ```ILogger``` extension methods for logging context data with exceptions
- ```Install-Package Divergic.Logging.NodaTime``` [on NuGet.org](https://www.nuget.org/packages/Divergic.Logging.NodaTime) contains an ```ILoggerFactory``` extension method to configure serialization support of NodaTime data types when adding exception context data

# Logging context data

Consider the following exception.

```csharp
public class PaymentGatewayException: Exception
{
    public PaymentGatewayException() : base("Failed to process complete transaction at the payment gateway.")
    {
    }
}
```

Logging the exception alone may not provide enough information to respond to a log entry. You can provide any context data you like with this package by using the ```LogErrorWithContext``` and ```LogCriticalWithContext``` extension methods on ```ILogger```.

```csharp
public async Task ProcessPayment(string invoiceId, int amountInCents, Person customer, CancellationToken cancellationToken)
{
    try
    {
        await _gateway.ProcessPayment(invoiceId, amountInCents, customer.Email, cancellationToken).ConfigureAwait(false);
    }
    catch (PaymentGatewayException ex)
    {
        var paymentDetails = new {
            invoiceId,
            amountInCents
        };
        
        _logger.LogErrorWithContext(ex, paymentDetails);
    }
}
```

This will append the context data to the ```Exception.Data``` property.

The context data appended to the exception will be JSON serialized as required. In the example above, the ```Exception.Data["ContextData"]``` entry would look something like the following.

```
"{\"invoiceId\":\"239349asfd-234234\",\"amountInCents\":3995}"
```

**NOTE:** Logging the ```Exception.Data``` property remains the responsibility of logging providers configured on the logger factory.

# Append additional data

The ```Divergic.Logging``` package also provides extension methods on ```Exception``` for adding additional data.

```csharp
public async Task ProcessPayment(string invoiceId, int amountInCents, Person customer, CancellationToken cancellationToken)
{
    try
    {
        await _gateway.ProcessPayment(invoiceId, amountInCents, customer.Email, cancellationToken).ConfigureAwait(false);
    }
    catch (PaymentGatewayException ex)
    {
        const string Key = "Customer";

        ex.AddSerializedData(Key, customer);
        
        _logger.LogError(ex);
    }
}
``` 

Adding data for the same key will ignore the request if the data has already been added. Need to check if serialized data is already there?

```csharp
public async Task ProcessPayment(string invoiceId, int amountInCents, Person customer, CancellationToken cancellationToken)
{
    try
    {
        await _gateway.ProcessPayment(invoiceId, amountInCents, customer.Email, cancellationToken).ConfigureAwait(false);
    }
    catch (PaymentGatewayException ex)
    {
        const string Key = "Customer";

        if (ex.HasSerializedData(Key) == false)
        {
            ex.AddSerializedData(Key, customer);
        }
        
        _logger.LogError(ex);
    }
}
``` 

# Support NodaTime serialization

Adding serialized context data to an exception is only as useful as how the data can be understood. ```NodaTime.Instant``` is a good example of this. The native JSON serialization of an Instant value will be ```{}``` rather than a readable date/time value. 

The Divergic.Logging.NodaTime package provides an extension method on ```ILoggerFactory``` to configure NodaTime support when adding exception context data.

```csharp
public void Configure(
    IApplicationBuilder app,
    IHostingEnvironment env,
    ILoggerFactory loggerFactory,
    IApplicationLifetime appLifetime)
{
    loggerFactory.UsingNodaTimeTypes();
}
```

Any NodaTime data type found will be correctly serialized when adding the context data to the exception.

# Customise exception context data serialization

You may need to make additional modifications to the JSON serialization behaviour when context data is serialized and added to an exception. You can do this by modifying the ```ExceptionData.SerializerSettings``` value.

```csharp
ExceptionData.SerializerSettings.MaxDepth = 15;
```
