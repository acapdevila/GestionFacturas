﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturas.Servicios
{  

    public static class ServicioEnumerable
    {
       public static string ObtenerNombreAtributoDisplay(this Enum value)
       {
           var enumType = value.GetType();
           var enumValue = Enum.GetName(enumType, value);

           if (enumValue == null) return string.Empty;

           var member = enumType.GetMember(enumValue)[0];

           var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);

           if (attrs.Length == 0) return enumValue;

           var outString =  ((DisplayAttribute)attrs[0]).Name;

           if (((DisplayAttribute)attrs[0]).ResourceType != null)
           {
               outString = ((DisplayAttribute)attrs[0]).GetName();
           }

           return outString;
       }

    }


}
