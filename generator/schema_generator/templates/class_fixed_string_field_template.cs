  private static readonly Lazy<nint> _$NAME$Offset = new(() => Schema.GetOffset($HASH$), LazyThreadSafetyMode.None);

  public string $NAME$ {
    get {
      var ptr = _Handle + _$NAME$Offset.Value;
      return Schema.GetString(ptr);
    }
    set => Schema.SetFixedString(_Handle, _$NAME$Offset.Value, value, $ELEMENT_COUNT$);
  } 