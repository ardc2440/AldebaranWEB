/**
 * Crea el Google Form de la encuesta.
 * Uso: script.google.com > Nuevo proyecto > pegar > Ejecutar crearEncuesta
 * (autorizar con la cuenta dueña del formulario). El log muestra los enlaces.
 */
function crearEncuesta() {
  var f = FormApp.create("Encuesta de procesos operativos – PROMOS");
  f.setDescription("Esta encuesta busca conocer cómo trabaja cada área en su día a día, para identificar oportunidades de mejora en las herramientas que apoyan la operación. No hay respuestas correctas o incorrectas; responda según su experiencia real. Las respuestas son confidenciales y se analizan de forma agrupada. Tiempo estimado: 10 minutos.");
  f.setCollectResponses(true); f.setShowLinkToRespondAgain(false);
  f.addTextItem().setTitle('Nombre (opcional)');
  f.addTextItem().setTitle('Cargo');
  f.addSectionHeaderItem().setTitle('Datos generales');
  f.addMultipleChoiceItem().setTitle("¿En qué área trabaja?").setChoiceValues(["Servicio al cliente (pedidos locales)", "Comercial / Asesora", "Comercio exterior", "Bodega / Inventario", "Producción / Personalización", "Despachos / Logística", "Gerencia / Dirección"]).showOtherOption(true).setRequired(true);
  f.addMultipleChoiceItem().setTitle("¿Con qué frecuencia usa el sistema Aldebaran?").setChoiceValues(["Todos los días", "Algunas veces por semana", "Rara vez", "No lo uso"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("¿Cuenta con celular inteligente con internet durante su jornada laboral?").setChoiceValues(["Sí", "No", "A veces"]).setRequired(true);
  f.addPageBreakItem().setTitle('Su operación diaria');
  f.addMultipleChoiceItem().setTitle("1. En un día normal, ¿cuántas consultas de clientes sobre el estado de sus pedidos atiende usted o su área?").setChoiceValues(["Ninguna", "1 a 5", "6 a 10", "11 a 20", "Más de 20", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("2. ¿Con qué frecuencia encuentran diferencias entre lo que muestra el sistema y lo que hay físicamente en bodega?").setChoiceValues(["Nunca", "Rara vez", "A veces", "Frecuentemente", "Siempre", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("3. Cuando un pedido pasa a personalización (marcación, sellado, etc.), ¿qué tan fácil es saber en qué etapa va?").setChoiceValues(["Muy fácil", "Fácil", "Regular", "Difícil", "Muy difícil", "No aplica a mi cargo"]).setRequired(true);
  f.addCheckboxItem().setTitle("4. ¿Cómo se entera su área del avance de una orden de compra antes de que llegue la mercancía? (puede marcar varias)").setChoiceValues(["Correo", "Llamada", "WhatsApp", "El proveedor avisa por iniciativa propia", "Solo cuando llega"]).showOtherOption(true).setRequired(true);
  f.addCheckboxItem().setTitle("5. ¿Cómo lleva el control de sus reservas y pedidos activos? (puede marcar varias)").setChoiceValues(["Consultando Aldebaran", "Excel propio", "Cuaderno o notas", "De memoria", "No aplica a mi cargo"]).showOtherOption(true).setRequired(true);
  f.addMultipleChoiceItem().setTitle("6. ¿Con qué frecuencia un cliente reclama que no recibió la mercancía o que llegó incompleta?").setChoiceValues(["Nunca", "Rara vez", "A veces", "Frecuentemente", "Siempre", "No aplica a mi cargo"]).setRequired(true);
  f.addParagraphTextItem().setTitle("7. ¿Qué tarea de su trabajo diario repite muchas veces y siente que podría hacerse de forma automática?");
  f.addMultipleChoiceItem().setTitle("8. ¿Los clientes piden el estado de su pedido por WhatsApp, por teléfono o fuera del horario laboral?").setChoiceValues(["Nunca", "Rara vez", "A veces", "Frecuentemente", "Siempre", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("9. ¿Cuánto tiempo toma un conteo completo del inventario?").setChoiceValues(["Menos de 1 día", "1 a 2 días", "3 a 5 días", "Más de una semana", "No se hace conteo completo", "No sé"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("10. ¿Con qué frecuencia un pedido se entrega después de la fecha pactada por demoras en la personalización?").setChoiceValues(["Nunca", "Rara vez", "A veces", "Frecuentemente", "Siempre", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("11. ¿Cuánto tiempo a la semana dedica a preguntar a los proveedores por el estado de sus órdenes?").setChoiceValues(["Nada", "Menos de 1 hora", "1 a 3 horas", "3 a 5 horas", "Más de 5 horas", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("12. En promedio, ¿cuánto tiempo le toma a una asesora saber qué tiene pendiente con cada uno de sus clientes (reservas, pedidos, entregas)?").setChoiceValues(["Lo sabe de inmediato", "Menos de 15 minutos", "15 a 30 minutos", "Más de 30 minutos", "No aplica a mi cargo"]).setRequired(true);
  f.addCheckboxItem().setTitle("13. ¿Cómo se programan los despachos del día? (puede marcar varias)").setChoiceValues(["Lista en Excel", "Correo", "De forma verbal", "Consultando Aldebaran", "WhatsApp", "No aplica a mi cargo"]).showOtherOption(true).setRequired(true);
  f.addParagraphTextItem().setTitle("14. ¿Qué información necesita para su trabajo y hoy le toca pedírsela a otra persona o área?");
  f.addMultipleChoiceItem().setTitle("15. En promedio, ¿cuánto se demora en ubicar físicamente una referencia en bodega?").setChoiceValues(["Menos de 5 minutos", "5 a 15 minutos", "15 a 30 minutos", "Más de 30 minutos", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("16. ¿Con qué frecuencia el cliente se entera de un retraso solo cuando pregunta o reclama?").setChoiceValues(["Nunca", "Rara vez", "A veces", "Frecuentemente", "Siempre", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("17. ¿Con cuántos talleres o satélites externos trabajan para personalizar productos?").setChoiceValues(["Ninguno, todo es interno", "1 a 2", "3 a 5", "Más de 5", "No sé"]).setRequired(true);
  f.addParagraphTextItem().setTitle("18. ¿Usa hojas de Excel u otros registros propios para complementar información que no le da el sistema? ¿Para qué?");
  f.addMultipleChoiceItem().setTitle("19. Cuando un proveedor retrasa una orden de compra, ¿cuánto tiempo pasa hasta que su área se entera?").setChoiceValues(["El mismo día", "1 a 3 días", "4 a 7 días", "Más de una semana", "Solo cuando la mercancía no llega", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("20. Cuando hay un reclamo por una entrega, ¿cuentan con evidencia (firma, foto, guía) para soportarlo?").setChoiceValues(["Siempre", "Casi siempre", "A veces", "Rara vez", "Nunca", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("21. Si es asesora o líder comercial: ¿cuánto tiempo al día dedica a revisar el estado de su cartera (reservas, pedidos, pendientes)?").setChoiceValues(["Menos de 15 minutos", "15 a 30 minutos", "30 a 60 minutos", "Más de 1 hora", "No aplica a mi cargo"]).setRequired(true);
  f.addParagraphTextItem().setTitle("22. ¿Hay algún proceso de su área que todavía se haga en papel? ¿Cuál?");
  f.addParagraphTextItem().setTitle("23. ¿Quién decide a qué persona o taller se asigna un trabajo de personalización, y cómo se comunica?");
  f.addMultipleChoiceItem().setTitle("24. ¿Cuántos despachos se realizan en un día normal?").setChoiceValues(["1 a 5", "6 a 15", "16 a 30", "Más de 30", "No sé"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("25. ¿Los proveedores suelen tener disposición para reportar el avance de sus órdenes por un medio digital?").setChoiceValues(["Sí, la mayoría", "Algunos", "Pocos", "No", "No sé"]).setRequired(true);
  f.addParagraphTextItem().setTitle("26. ¿Qué reporte o información le gustaría tener y hoy no existe?");
  f.addMultipleChoiceItem().setTitle("27. ¿Sus clientes suelen comunicarse con Promos por medios digitales (correo, página web, WhatsApp)?").setChoiceValues(["Nunca", "Rara vez", "A veces", "Frecuentemente", "Siempre", "No aplica a mi cargo"]).setRequired(true);
  f.addMultipleChoiceItem().setTitle("28. ¿Con qué frecuencia se realizan ajustes de inventario por faltantes o sobrantes encontrados?").setChoiceValues(["Nunca", "Rara vez", "A veces", "Frecuentemente", "Siempre", "No aplica a mi cargo"]).setRequired(true);
  f.addCheckboxItem().setTitle("29. ¿Cuáles de estas tareas le consumen más tiempo? (puede marcar varias)").setChoiceValues(["Responder consultas de clientes", "Hacer seguimiento a proveedores", "Buscar mercancía en bodega", "Coordinar despachos", "Hacer seguimiento a la personalización", "Preparar informes", "Digitar información en varios sistemas"]).showOtherOption(true).setRequired(true);
  f.addParagraphTextItem().setTitle("30. ¿En qué épocas del año se satura su área y qué es lo que más se complica?");
  f.addParagraphTextItem().setTitle("31. Si pudiera cambiar una sola cosa de la forma en que trabaja su área, ¿cuál sería?");
  Logger.log('Editar: ' + f.getEditUrl());
  Logger.log('Responder: ' + f.getPublishedUrl());
}
