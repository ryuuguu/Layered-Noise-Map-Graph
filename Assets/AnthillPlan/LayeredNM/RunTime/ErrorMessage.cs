using System;
using System.Collections.Generic;

namespace AnthillPlan.LayeredNM {
  [Serializable]
  public class ErrorMessage {
    public bool isError;
    public List<string> messages;
    public string message; //used by custom Inspector
   
  }
}