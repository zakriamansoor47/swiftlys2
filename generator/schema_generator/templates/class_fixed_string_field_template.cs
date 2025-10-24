  public string $NAME$ {
    get {
      var ptr = _Handle + Schema.GetOffset($HASH$);
      return Schema.GetString(ptr);
    }
    set => Schema.SetFixedString(_Handle, $HASH$, value, $ELEMENT_COUNT$);
  } 