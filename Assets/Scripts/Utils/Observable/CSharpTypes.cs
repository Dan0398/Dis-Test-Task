namespace Utils.Observables
{
    [System.Serializable]
    public class ObsInt : Observable<int>
    {
        public ObsInt(int Value) : base(Value) { }

        public static implicit operator ObsInt(int value) => new ObsInt(value);
    }

    [System.Serializable]
    public class ObsFloat : Observable<float>
    {
        public ObsFloat(float Value) : base(Value) { }

        public static implicit operator ObsFloat(float value) => new ObsFloat(value);
    }

    [System.Serializable]
    public class ObsString: Observable<string>
    {
        public ObsString(string Value) : base(Value){}
        
        public static implicit operator ObsString(string value) => new ObsString(value); 
    }
    
    [System.Serializable]
    public class ObsBool: Observable<bool>
    {
        public ObsBool(bool Value) : base(Value){}
        
        public static implicit operator ObsBool(bool value) => new ObsBool(value); 
    }
}