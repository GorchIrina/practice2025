using System;
using System.Linq;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input){
        if(string.IsNullOrEmpty(input)){
            return false;
        }
        input=input.ToLower();
        char[] chars = input.ToCharArray();
        char[] cleanedChars= chars.Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c)).ToArray();
        string cleanedInp=new string(cleanedChars); 
        if(string.IsNullOrEmpty(cleanedInp)){
            return false;
        }
        Array.Reverse(cleanedChars);
        string revInp=new string(cleanedChars);    
        return cleanedInp==revInp;
    }
}
