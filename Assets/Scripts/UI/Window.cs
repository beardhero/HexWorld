using System.Collections;
using System.Collections.Generic;

public abstract class Window {

	protected GUIManager manager;

	public abstract void Update();

	public abstract void Render();
}
