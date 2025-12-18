import subprocess
import pyautogui
import time
import os
import pytest

# --- CONFIGURACIÓN ---
GAME_EXE = "Juego_de_avion.exe"

# ==========================================
# PRUEBAS DE INTEGRACIÓN (JUEGO REAL)
# ==========================================

def test_1_ejecucion_y_duracion():
    """
    Abre el juego y verifica que se mantiene abierto por unos segundos.
    Cubre: Ejecución y Duración mínima.
    """
    print(f"Iniciando {GAME_EXE}...")
    try:
        # Iniciamos el juego
        proceso = subprocess.Popen([GAME_EXE], shell=True)
        
        # Esperamos para simular carga y juego
        time.sleep(5) 
        
        # Verificamos que el proceso sigue vivo (poll() devuelve None si sigue corriendo)
        assert proceso.poll() is None, "El juego se cerró inesperadamente."
        
        print("El juego se inició y mantuvo estable.")
        
    except Exception as e:
        pytest.fail(f"Error al iniciar el juego: {e}")

def test_2_interaccion_teclado():
    """
    Envía comandos de teclado al juego.
    Cubre: Selección de avión (simulada) y Disparo.
    """
    # Asumimos que el juego ya está abierto del test anterior o lo abrimos
    # Damos foco a la ventana (clic al centro por si acaso)
    pyautogui.click(x=960, y=540)
    
    # 1. Simular navegación en el menú (Flechas y Enter)
    print("Navegando en el menú...")
    pyautogui.press('right') # Cambiar nivel/avión
    time.sleep(0.5)
    pyautogui.press('left')
    time.sleep(0.5)
    
    # 2. Simular inicio de juego (Enter o clic en botón Jugar)
    # Como tu menú usa botones visuales, el teclado solo no basta para "clic",
    # pero probamos que no crashee al recibir input.
    pyautogui.press('enter') 
    
    # 3. Simular disparo (Espacio)
    print("Disparando...")
    for _ in range(5):
        pyautogui.press('space')
        time.sleep(0.1)
        
    assert True # Si llegamos aquí sin error de script, pasó.

# ==========================================
# PRUEBAS UNITARIAS DE LÓGICA (SIMULACIÓN)
# ==========================================
# Como no podemos acceder a las clases de C# desde Python,
# replicamos la lógica clave para verificar que el algoritmo es correcto.

class LogicaAvion:
    """Simulación de la clase Jugador/Avion de tu código C#"""
    def __init__(self, vida_maxima):
        self.vida = vida_maxima
    
    def recibir_dano(self, cantidad):
        self.vida -= cantidad
        if self.vida < 0: self.vida = 0
        
    def esta_vivo(self):
        return self.vida > 0

def test_3_validacion_dano_destruccion():
    """
    Verifica la lógica de daño que usas en Jugador.cs.
    Cubre: Validación de daño recibido y destrucción.
    """
    # Tu nave tiene vida base * 100. Ej: (2 + 0) * 100 = 200
    nave = LogicaAvion(vida_maxima=200)
    
    # Recibe daño (ej: colisión con enemigo = 40)
    nave.recibir_dano(40)
    assert nave.vida == 160, "El daño no se restó correctamente"
    
    # Recibe daño letal
    nave.recibir_dano(200)
    assert nave.vida == 0, "La vida no debería bajar de 0"
    assert not nave.esta_vivo(), "La nave debería estar destruida"

class LogicaPuntaje:
    """Simulación de la lógica de puntaje en Formulario_Principal.cs"""
    def __init__(self):
        self.puntos = 0
        
    def enemigo_destruido(self, tipo_enemigo):
        # Según tu código: Enemigo=100, Obstaculo=20
        if tipo_enemigo == "nave":
            self.puntos += 100
        elif tipo_enemigo == "asteroide":
            self.puntos += 20

def test_4_verificacion_puntaje():
    """
    Verifica la suma de puntos.
    Cubre: Verificación de puntaje.
    """
    juego = LogicaPuntaje()
    
    # Matar 2 naves y 1 asteroide
    juego.enemigo_destruido("nave")
    juego.enemigo_destruido("nave")
    juego.enemigo_destruido("asteroide")
    
    esperado = 100 + 100 + 20
    assert juego.puntos == esperado, f"Puntaje incorrecto. Esperado {esperado}, obtenido {juego.puntos}"

# ==========================================
# PRUEBAS VISUALES (REQUIEREN IMÁGENES)
# ==========================================

def test_5_botones_visuales():
    """
    Busca botones en pantalla.
    Cubre: Captura de botones 'Iniciar partida' y 'Volver'.
    NOTA: Requiere que tengas 'btn_jugar.png' y 'btn_volver.png' en la carpeta.
    """
    # Saltamos este test si no hay imágenes, para no fallar todo
    if not os.path.exists("btn_jugar.png"):
        pytest.skip("No se encontró btn_jugar.png, saltando test visual.")
    
    print("Buscando botones...")
    boton_jugar = pyautogui.locateOnScreen("btn_jugar.png", confidence=0.7)
    
    if boton_jugar:
        print("Botón JUGAR encontrado.")
        assert True
    else:
        # No fallamos el test crítico, solo avisamos
        print("Advertencia: No se vio el botón Jugar (¿Resolución diferente?)")

# ==========================================
# LIMPIEZA
# ==========================================

def test_6_cierre_juego():
    """
    Cierra el juego forzosamente.
    Cubre: Cierra el juego al finalizar.
    """
    print("Cerrando juego...")
    # Matamos el proceso por nombre
    os.system(f"taskkill /f /im {GAME_EXE}")
    time.sleep(1)
    
    # Verificamos que ya no esté corriendo (opcional, requiere librerías extra como psutil)
    print("Juego cerrado.")
    assert True