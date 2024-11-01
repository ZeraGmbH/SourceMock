using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;

namespace BurdenApi.Actions;

/// <summary>
/// 
/// </summary>
/// <param name="port"></param>
#pragma warning disable CS9113 // Parameter is unread.
public class Burden([FromKeyedServices("Burden")] ISerialPortConnection port) : IBurden { }
#pragma warning restore CS9113 // Parameter is unread.